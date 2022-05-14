using General;
using Scoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using UnityEngine;

public enum EInterviewState
{
    Setup,
    SelectingSubjects,
    NoSubject,
    SubjectEntry,
    Confession,
    AwaitingSentenceInput,
    ExecutingSentence,
    ReviewAwaiting,
    Review,
    InterviewDone,
}

[Serializable]
public class SubjectResult
{
    public SubjectResult(SubjectInformation info, int sentence)
    {
        subjectInformation = info;
        sentenceValue = sentence;
    }

    public SubjectInformation subjectInformation;
    public int sentenceValue;
    public float averageSentenceValue;
}

public delegate void OnInterviewProgressionDelegate(SubjectInformation currentSubject, EInterviewState interviewState);

public delegate void OnInterviewFinished(List<SubjectResult> results);

public class Interview : MonoBehaviour
{
    public OnInterviewProgressionDelegate onReviewProgression;
    public OnInterviewProgressionDelegate onInterviewProgression;
    public OnInterviewFinished onInterviewFinished;
    public List<SubjectInformation> Subjects = new();

    public List<SubjectInformation> SubjectsToJudge = new();
    [NonSerialized] [ReadOnlyField] public List<SubjectInformation> JudgedSubjects = new();

    public SubjectInformation currentSubject = null;
    // Start is called before the first frame update

    public EInterviewState interviewState = EInterviewState.Setup;
    public int numSubjectsToJudge = 5;

    public List<SubjectResult> results = new();
    private Dictionary<string, float> _globalScores = new();

    private bool _hasFetchedScores = false;

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
        JudgedSubjects.Add(currentSubject);

        var oldInterviewState = interviewState;
        if (SubjectsToJudge.Any())
        {
            interviewState = EInterviewState.NoSubject;
        }
        else if (interviewState != EInterviewState.Review)
        {
            interviewState = EInterviewState.ReviewAwaiting;
        }
        else
        {
            interviewState = EInterviewState.Review;
        }

        currentSubject = null;

        if (interviewState == EInterviewState.ReviewAwaiting)
        {
            onInterviewFinished?.Invoke(results);

            // currentSubject = JudgedSubjects[0];
            // JudgedSubjects.RemoveAt(0);
            // onReviewProgression?.Invoke(currentSubject, EInterviewState.Review);
        }

        onInterviewProgression?.Invoke(currentSubject, interviewState);
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
        switch (request)
        {
            case PlayerRequests.Login:
                onInterviewProgression?.Invoke(null, EInterviewState.SelectingSubjects);
                break;
            case PlayerRequests.NextSubject:
            {
                Debug.Assert(currentSubject == null);
                Debug.Assert(interviewState == EInterviewState.NoSubject);
                Debug.Assert(numSubjectsToJudge > 0);
                Debug.Assert(SubjectsToJudge.Count > 0);

                if (value == 0 && !_hasFetchedScores)
                {
                    // First, load all scores
                    FetchScores();
                }

                if (SubjectsToJudge.Any())
                {
                    currentSubject = SubjectsToJudge[0];
                    SubjectsToJudge.RemoveAt(0);
                }

                interviewState = EInterviewState.SubjectEntry;
                onInterviewProgression?.Invoke(currentSubject, interviewState);
                break;
            }
            case PlayerRequests.Sentence:
                Debug.Assert(currentSubject != null);
                Debug.Assert(interviewState == EInterviewState.AwaitingSentenceInput);
                interviewState = EInterviewState.ExecutingSentence;

                var subjectResult = new SubjectResult(currentSubject, value);
                if (_globalScores.TryGetValue(currentSubject.subjectName, out var globalScore))
                {
                    subjectResult.averageSentenceValue = globalScore;
                }

                results.Add(subjectResult);

                Hub.Get<ScoringController>().PostScore(currentSubject.subjectName, value);

                onInterviewProgression?.Invoke(currentSubject, interviewState);
                break;
            case PlayerRequests.Review:

                if (JudgedSubjects.Any())
                {
                    currentSubject = JudgedSubjects[0];
                    JudgedSubjects.RemoveAt(0);

                    if (interviewState == EInterviewState.ReviewAwaiting)
                    {
                        interviewState = EInterviewState.Review;
                        onInterviewProgression?.Invoke(currentSubject, interviewState);
                    }

                    onReviewProgression?.Invoke(currentSubject, EInterviewState.Review);
                }
                else
                {
                    onInterviewProgression?.Invoke(null, EInterviewState.InterviewDone);
                }

                break;
            case PlayerRequests.SkipReview:
                onInterviewProgression?.Invoke(null, EInterviewState.InterviewDone);
                break;
        }
    }

    private async Task FetchScores()
    {
        _hasFetchedScores = true;
        _globalScores = await Hub.Get<ScoringController>().GetScores();
    }
}