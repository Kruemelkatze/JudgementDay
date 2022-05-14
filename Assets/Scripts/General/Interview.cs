using General;
using Scoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EInterviewState
{
    Setup,
    SelectingSubjects,
    NoSubject,
    SubjectEntry,
    Confession,
    AwaitingSentenceInput,
    ExecutingSentence,
    InterviewDone
}

[System.Serializable]
public struct SubjectResult
{
    public SubjectResult(SubjectInformation info, int sentence)
    {
        subjectInformation = info;
        sentenceValue = sentence;
    }

    public SubjectInformation subjectInformation;
    public int sentenceValue;
}

public delegate void OnInterviewProgressionDelegate(SubjectInformation currentSubject, EInterviewState interviewState);

public delegate void OnInterviewFinished(List<SubjectResult> results);

public class Interview : MonoBehaviour
{
    public OnInterviewProgressionDelegate onInterviewProgression;
    public OnInterviewFinished onIntervieFinished;
    public List<SubjectInformation> Subjects = new List<SubjectInformation>();

    public List<SubjectInformation> SubjectsToJudge = new List<SubjectInformation>();

    public SubjectInformation currentSubject = null;
    // Start is called before the first frame update

    public EInterviewState interviewState = EInterviewState.Setup;
    public int numSubjectsToJudge = 5;

    public List<SubjectResult> results = new List<SubjectResult>();

    private void Awake()
    {
        numSubjectsToJudge = Math.Min(Subjects.Count, numSubjectsToJudge);
        SelectRandomSubjects();
        Hub.Register<Interview>(this);
    }

    void Start()
    {
        Hub.Get<VideoBackground>().onSubjectClipCompleted += NotifySubjectClipCompleted;
        GameController player = Hub.Get<GameController>();
        player.onPlayerRequest += NotifyPlayerRequest;
        StartCoroutine(StartGameAfterDelay(1.0f));
    }

    public List<SubjectInformation> SelectRandomSubjects()
    {
        return SubjectsToJudge = Subjects.OrderBy(x => Guid.NewGuid()).Take(numSubjectsToJudge)
            .OrderBy(x => Subjects.IndexOf(x)).ToList();
    }

    IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        interviewState = EInterviewState.NoSubject;
        onInterviewProgression?.Invoke(null, interviewState);
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
                //nothing really, we wait for player to input something
                break;
            case ESubjectClipType.Sentenced:
                SubjectSentencedClipCompleted();
                break;
        }
    }

    private void SubjectSentencedClipCompleted()
    {
        interviewState = SubjectsToJudge.Any() ? EInterviewState.NoSubject : EInterviewState.InterviewDone;
        currentSubject = null;
        onInterviewProgression?.Invoke(currentSubject, interviewState);

        if (interviewState == EInterviewState.InterviewDone)
        {
            onIntervieFinished?.Invoke(results);
        }
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
        if (request == PlayerRequests.NextSubject)
        {
            Debug.Assert(currentSubject == null);
            Debug.Assert(interviewState == EInterviewState.NoSubject);
            Debug.Assert(numSubjectsToJudge > 0);
            Debug.Assert(SubjectsToJudge.Count > 0);

            if (SubjectsToJudge.Any())
            {
                currentSubject = SubjectsToJudge[0];
                SubjectsToJudge.RemoveAt(0);
            }

            interviewState = EInterviewState.SubjectEntry;
            onInterviewProgression?.Invoke(currentSubject, interviewState);
        }
        else if (request == PlayerRequests.Sentence)
        {
            Debug.Assert(currentSubject != null);
            Debug.Assert(interviewState == EInterviewState.AwaitingSentenceInput);
            interviewState = EInterviewState.ExecutingSentence;

            results.Add(new SubjectResult(currentSubject, value));

            Hub.Get<ScoringController>().PostScore(currentSubject.subjectName, value);

            onInterviewProgression?.Invoke(currentSubject, interviewState);
        }
    }
}