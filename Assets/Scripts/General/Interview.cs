using General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInterviewState
{
    NoSubject,
    SubjectIntro,
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

    EInterviewState interviewState = EInterviewState.NoSubject;
    public int numSubjectsToJudge = 5;

    private void Awake()
    {
        Hub.Register<Interview>(this);
    }
    void Start()
    {
        Hub.Get<VideoBackground>().onSubjectClipCompleted += NotifySubjectClipCompleted;
        GameController player = Hub.Get<GameController>();
        player.onPlayerRequest += NotifyPlayerRequest;
    }

    private void NotifySubjectClipCompleted(SubjectClipData clip, bool isLoopingClip)
    {
        switch (clip.clipType)
        {
            case ESubjectClipType.Intro:
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

    private void GetNextSubject()
    {

    }

    private void SubjectIdleClipCompleted()
    {

    }

    private void SubjectInterviewClipCompleted()
    {

    }

    private void SubjectIntroClipCompleted()
    {

    }
    private void NotifyPlayerRequest(PlayerRequests request)
    {
        if(request == PlayerRequests.NextSubject)
        {
            Debug.Assert(currentSubject == null);
            Debug.Assert(interviewState == EInterviewState.NoSubject);
            if (Subjects.Count > 0)
            {
                currentSubject = Subjects[0];
                Subjects.RemoveAt(Subjects.Count);
                interviewState = EInterviewState.SubjectIntro;
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
}
