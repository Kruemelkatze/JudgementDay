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

public class VirtualScreenViewModel : MonoBehaviour
{
    public GameObject BootupScreen;
    public GameObject SubjectSelectionScreen;
    public GameObject PersonScreen;
    public GameObject PenaltyScreen;
    public GameObject PunishmentSuccessfulScreen;
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

    private void Awake()
    {
        Hub.Register<VirtualScreenViewModel>(this);
    }

    private void Start()
    {
        BootupScreen.SetActive(true);
        SubjectSelectionScreen.SetActive(false);
        PersonScreen.SetActive(false);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(false);
        AllPunishmentSuccessfulScreen.SetActive(false);
        JudgeNowButton.interactable = false;
        Hub.Get<Interview>().onInterviewProgression += NotifyInterviewProgression;
        Hub.Get<Interview>().onIntervieFinished += NotifyInterviewFinished;
    }

    private void NotifyInterviewFinished(List<SubjectResult> results)
    {
        // score screen
    }

    public void Login()
    {
        FillSubjects();

        BootupScreen.SetActive(false);
        SubjectSelectionScreen.SetActive(true);
        PersonScreen.SetActive(false);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(false);
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
        Hub.Get<GameController>().RequestNextSubject();
        SubjectSelectionScreen.SetActive(false);
        BootupScreen.SetActive(false);
        PersonScreen.SetActive(true);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToJudgeScreen()
    {
        PersonScreen.SetActive(false);
        PenaltyScreen.SetActive(true);
    }

    public void JudgementGoBack()
    {
        PersonScreen.SetActive(true);
        PenaltyScreen.SetActive(false);
        PenaltySlider.value = 1;
    }

    public void JudgeNow()
    {
        BootupScreen.SetActive(false);
        PersonScreen.SetActive(false);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(true);
        Hub.Get<GameController>().ChooseSentence((int)PenaltySlider.value);
    }

    private void JudgeNow(int chosenSentenceIndex)
    {
        Hub.Get<GameController>().ChooseSentence(chosenSentenceIndex);
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
            case EInterviewState.SubjectEntry:
                BootupScreen.SetActive(false);
                PersonScreen.SetActive(true);
                SubjectName.SetText(currentsubject.subjectName);
                SubjectCountry.SetText("Country: " + currentsubject.country);
                SubjectDeathDate.SetText("Death: " + currentsubject.deathDate);
                SubjectDescription.SetText(currentsubject.informationText);
                SubjectImage.sprite = currentsubject.sprite;
                break;
            case EInterviewState.InterviewDone:
                BootupScreen.SetActive(false);
                PersonScreen.SetActive(false);
                PenaltyScreen.SetActive(false);
                PunishmentSuccessfulScreen.SetActive(false);
                AllPunishmentSuccessfulScreen.SetActive(true);
                JudgementSuccessfulButton.interactable = true;
                break;
        }
    }
}