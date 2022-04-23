using General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EInterviewState
{
    NoSubject,
    SubjectEntry,
    Confession,
    AwaitingSentenceInput,
    ExecutingSentence
}

public delegate void OnInterviewProgressionDelegate(SubjectInformation currentSubject, EInterviewState interviewState);
public class Interview : MonoBehaviour
{
    public OnInterviewProgressionDelegate onInterviewProgression;
    public List<SubjectInformation> Subjects = new List<SubjectInformation>();
    public SubjectInformation currentSubject = null;
    // Start is called before the first frame update

    public EInterviewState interviewState = EInterviewState.NoSubject;
    public int numSubjectsToJudge = 5;

    public GameObject entrySmokeParticles;
    public GameObject sentenceFireParticles;

    private void Awake()
    {
        numSubjectsToJudge = Math.Min(Subjects.Count, numSubjectsToJudge);
        Hub.Register<Interview>(this);
    }
    void Start()
    {
        Hub.Get<VideoBackground>().onSubjectClipCompleted += NotifySubjectClipCompleted;
        GameController player = Hub.Get<GameController>();
        player.onPlayerRequest += NotifyPlayerRequest;
        StartCoroutine(StartGameAfterDelay(1.0f));
    }

    IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NotifyPlayerRequest(PlayerRequests.NextSubject, 0);
    }

    private void NotifySubjectClipCompleted(SubjectClipData clip, bool isLoopingClip)
    {
        switch (clip.clipType)
        {
            case ESubjectClipType.Entry:
                SubjectIntroClipCompleted();
                break;
            case ESubjectClipType.Confession:
                SubjectInterviewClipCompleted();
                break;
            case ESubjectClipType.AwaitSentencing:
                SubjectIdleClipCompleted();
                break;
            case ESubjectClipType.Sentenced:
                SubjectSentencedClipCompleted();
                break;
        }
    }

    private void SubjectSentencedClipCompleted()
    {
        interviewState = EInterviewState.NoSubject;
        currentSubject = null;
        onInterviewProgression?.Invoke(currentSubject, interviewState);
    }

    private void SubjectIdleClipCompleted()
    {
        //nothing really, we wait for player to input something
    }

    private void SubjectInterviewClipCompleted()
    {
        interviewState = EInterviewState.AwaitingSentenceInput;
        onInterviewProgression?.Invoke(currentSubject, interviewState);
    }

    private void SubjectIntroClipCompleted()
    {
        interviewState = EInterviewState.Confession;
        onInterviewProgression?.Invoke(currentSubject, interviewState);
    }
    private void NotifyPlayerRequest(PlayerRequests request, int value)
    {
        if(request == PlayerRequests.NextSubject)
        {
            Debug.Assert(currentSubject == null);
            Debug.Assert(interviewState == EInterviewState.NoSubject);
            if (Subjects.Count > 0)
            {
                int rngindex = Random.Range(0, Subjects.Count);
                currentSubject = Subjects[rngindex];
                Subjects.RemoveAt(rngindex);
                interviewState = EInterviewState.SubjectEntry;
                onInterviewProgression?.Invoke(currentSubject, interviewState);
            }
        }
        else if(request == PlayerRequests.Sentence)
        {
            Debug.Assert(currentSubject != null);
            Debug.Assert(interviewState == EInterviewState.AwaitingSentenceInput);
            interviewState = EInterviewState.ExecutingSentence;
            onInterviewProgression?.Invoke(currentSubject, interviewState);
        }
    }

    void PlayFireParticles()
    {
        ParticleSystem ps = sentenceFireParticles.GetComponent<ParticleSystem>();
        
    }
}
