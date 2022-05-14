using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using General;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class VirtualScreenViewModel : MonoBehaviour
{
    public GameObject BootupScreen;
    public GameObject SubjectSelectionScreen;
    public GameObject PersonScreen;
    public GameObject PenaltyScreen;
    public GameObject PunishmentSuccessfulScreen;
    public GameObject ReviewAwaitingScreen;
    public GameObject ReviewScreen;
    public GameObject AllPunishmentSuccessfulScreen;

    [Space] public TextMeshProUGUI SubjectName;
    public TextMeshProUGUI SubjectCountry;
    public TextMeshProUGUI SubjectDeathDate;
    public TextMeshProUGUI SubjectDescription;
    public Image SubjectImage;
    public Button JudgeNowButton;
    public Button JudgementSuccessfulButton;
    public Slider PenaltySlider;
    public GridLayoutGroup subjectLayoutGroup;
    public Button startJudgeButton;

    [Space] public GameObject subjectInfoUIPrefab;

    [Space] public TextMeshProUGUI ReviewSubjectName;
    public TextMeshProUGUI ReviewSubjectShortDescription;
    public Image ReviewSubjectImage;
    public Transform ReviewSubjectPlayerPunishmentContainer;
    public Transform ReviewSubjectAveragePunishmentContainer;
    public TextMeshProUGUI ReviewSubjectRealPenalty;

    private Dictionary<SubjectInformation, SubjectResult> _results;

    private void Awake()
    {
        Hub.Register<VirtualScreenViewModel>(this);
    }

    private void Start()
    {
        ShowScreen(BootupScreen);
        JudgeNowButton.interactable = false;
        Hub.Get<Interview>().onInterviewProgression += NotifyInterviewProgression;
        Hub.Get<Interview>().onReviewProgression += NotifyReviewProgression;
        Hub.Get<Interview>().onInterviewFinished += NotifyInterviewFinished;
    }

    private void NotifyInterviewFinished(List<SubjectResult> results)
    {
        // score screen
        _results = results.ToDictionary(s => s.subjectInformation);
    }

    public void Login()
    {
        FillSubjects();

        ShowScreen(SubjectSelectionScreen);
        Hub.Get<GameController>().Login();
    }

    private void FillSubjects()
    {
        foreach (Transform child in subjectLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        var interview = Hub.Get<Interview>();
        if (interview.SubjectsToJudge == null || !interview.SubjectsToJudge.Any())
        {
            interview.SelectRandomSubjects();
        }

        var subjects = interview.Subjects;
        foreach (var subject in subjects)
        {
            var subjectInfoUi = Instantiate(subjectInfoUIPrefab, subjectLayoutGroup.transform);

            var subjectInfoUiScript = subjectInfoUi.GetComponent<SubjectInfoUI>();
            subjectInfoUiScript.SetSubjectInfo(subject);
            subjectInfoUiScript.OnClicked += SubjectInfoClicked;

            LayoutRebuilder.ForceRebuildLayoutImmediate(subjectLayoutGroup.transform as RectTransform);
        }

        UpdateSubjectHighlight();
    }

    private void SubjectInfoClicked(SubjectInfoUI obj)
    {
        var interview = Hub.Get<Interview>();
        if (interview.SubjectsToJudge.Contains(obj.subjectInfo))
        {
            interview.SubjectsToJudge.Remove(obj.subjectInfo);
            obj.SetHighlighted(false);
        }
        else
        {
            interview.SubjectsToJudge.Add(obj.subjectInfo);
            interview.SubjectsToJudge = interview.SubjectsToJudge.OrderBy(x => interview.Subjects.IndexOf(x)).ToList();
            obj.SetHighlighted(true);
        }

        startJudgeButton.interactable = interview.SubjectsToJudge.Any();
    }

    private void UpdateSubjectHighlight()
    {
        var interview = Hub.Get<Interview>();

        foreach (Transform child in subjectLayoutGroup.transform)
        {
            var subjectInfoUiScript = child.GetComponent<SubjectInfoUI>();
            if (!subjectInfoUiScript)
                continue;

            var selected = interview.SubjectsToJudge.Contains(subjectInfoUiScript.subjectInfo);
            subjectInfoUiScript.SetHighlighted(selected);
        }
    }

    public void StartJudging()
    {
        JudgementSuccessfulButton.interactable = false;
        JudgeNowButton.interactable = false;
        Hub.Get<GameController>().RequestSubject();
        PenaltySlider.value = Random.Range(PenaltySlider.minValue, PenaltySlider.maxValue + 1);
        ShowScreen(PersonScreen);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToJudgeScreen()
    {
        ShowScreen(PenaltyScreen);
    }

    public void JudgementGoBack()
    {
        ShowScreen(PersonScreen);
    }

    public void JudgeNow()
    {
        ShowScreen(PunishmentSuccessfulScreen);
        Hub.Get<GameController>().ChooseSentence((int)PenaltySlider.value);
    }

    private void JudgeNow(int chosenSentenceIndex)
    {
        Hub.Get<GameController>().ChooseSentence(chosenSentenceIndex);
    }

    private void ShowScreen(GameObject screen)
    {
        BootupScreen.SetActive(false);
        SubjectSelectionScreen.SetActive(false);
        PersonScreen.SetActive(false);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(false);
        ReviewAwaitingScreen.SetActive(false);
        ReviewScreen.SetActive(false);
        AllPunishmentSuccessfulScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void NextReview()
    {
        Hub.Get<GameController>().RequestNextReview();
    }

    public void SkipReview()
    {
        Hub.Get<GameController>().RequestSkipReview();
    }

    private void NotifyReviewProgression(SubjectInformation currentsubject, EInterviewState interviewState)
    {
        ReviewSubjectName.text = currentsubject.subjectName;
        ReviewSubjectShortDescription.text = currentsubject.shortVersion;
        ReviewSubjectImage.sprite = currentsubject.sprite;
        ReviewSubjectRealPenalty.text = currentsubject.realSentence;

        if (_results.TryGetValue(currentsubject, out var result))
        {
            EnableNthChild(ReviewSubjectPlayerPunishmentContainer, result.sentenceValue - 1);
            EnableNthChild(ReviewSubjectAveragePunishmentContainer,
                (int)Mathf.Round(result.averageSentenceValue) - 1);
        }

        ShowScreen(ReviewScreen);
    }

    private void NotifyInterviewProgression(SubjectInformation currentsubject, EInterviewState interviewstate)
    {
        switch (interviewstate)
        {
            case EInterviewState.NoSubject:
                JudgementSuccessfulButton.interactable = true;
                break;
            case EInterviewState.Confession:
                JudgeNowButton.interactable = false;
                break;
            case EInterviewState.AwaitingSentenceInput:
                JudgeNowButton.interactable = true;
                break;
            case EInterviewState.ExecutingSentence:
                break;
            case EInterviewState.ReviewAwaiting:
                ShowScreen(ReviewAwaitingScreen);
                break;
            case EInterviewState.SubjectEntry:
                ShowScreen(PersonScreen);
                SubjectName.SetText(currentsubject.subjectName);
                SubjectCountry.SetText("Country: " + currentsubject.country);
                SubjectDeathDate.SetText("Death: " + currentsubject.deathDate);
                SubjectDescription.SetText(currentsubject.informationText);
                SubjectImage.sprite = currentsubject.sprite;
                break;
            case EInterviewState.InterviewDone:
                ShowScreen(AllPunishmentSuccessfulScreen);
                JudgementSuccessfulButton.interactable = true;
                break;
        }
    }

    private void EnableNthChild(Transform container, int child)
    {
        var index = Math.Clamp(child, 0, container.childCount);

        foreach (Transform ct in container.transform)
        {
            ct.gameObject.SetActive(false);
        }

        if (container.childCount > index)
        {
            container.GetChild(index).gameObject.SetActive(true);
        }
        else if (container.childCount == 1)
        {
            container.GetChild(0).gameObject.SetActive(true);
        }
    }
}