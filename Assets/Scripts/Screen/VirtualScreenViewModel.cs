using System;
using System.Collections;
using System.Collections.Generic;
using General;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class VirtualScreenViewModel : MonoBehaviour
{
    public GameObject BootupScreen;
    public GameObject PersonScreen;
    public GameObject PenaltyScreen;
    public GameObject PunishmentSuccessfulScreen;
    public GameObject AllPunishmentSuccessfulScreen;

    public TextMeshProUGUI SubjectName;
    public TextMeshProUGUI SubjectCountry;
    public TextMeshProUGUI SubjectDeathDate;
    public TextMeshProUGUI SubjectDescription;
    public Image SubjectImage;
    public Button JudgeNowButton;
    public Button JudgementSuccessfulButton;
    public Slider PenaltySlider;
    
    
    private void Awake()
    {
        Hub.Register<VirtualScreenViewModel>(this);
    }

    private void Start()
    {
        BootupScreen.SetActive(true);
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

    public void StartJudging()
    {
        JudgementSuccessfulButton.interactable = false;
        JudgeNowButton.interactable = false;
        Hub.Get<GameController>().RequestNextSubject();
        BootupScreen.SetActive(false);
        PersonScreen.SetActive(true);
        PenaltyScreen.SetActive(false);
        PunishmentSuccessfulScreen.SetActive(false);
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
        Hub.Get<GameController>().ChooseSentence((int) PenaltySlider.value);
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
                StartCoroutine("startNewGame");
                break;
        }
    }

    private IEnumerator startNewGame()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("2DScene");
    } 
}
