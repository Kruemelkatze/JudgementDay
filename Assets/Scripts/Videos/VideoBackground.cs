using General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Videos;

public delegate void OnSubjectClipCompletedDelegate(SubjectClipData clip, bool isLoopingClip);

[RequireComponent(typeof(VideoClipAssetPlayer))]
[RequireComponent(typeof(VideoPlayer))]
public class VideoBackground : MonoBehaviour
{
    public OnSubjectClipCompletedDelegate onSubjectClipCompleted;

    public VideoPlayer videoPlayer;
    public VideoClipAssetPlayer vcap;

    public SubjectInformation currentSubject = null;

    public SubjectClipData currentClipData = null;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        vcap = GetComponent<VideoClipAssetPlayer>();
        videoPlayer.prepareCompleted += NotifyVideoPrepareCompleted;
        videoPlayer.loopPointReached += NotifyClipLoopPointReached;
        Hub.Register<VideoBackground>(this);
    }

    private void Start()
    {
        Hub.Get<Interview>().onInterviewProgression += NotifyInterviewProgression;
    }

    private void NotifyInterviewProgression(SubjectInformation currentSubject, EInterviewState interviewState)
    {
        switch(interviewState)
        {
            case EInterviewState.ExecutingSentence:
                PlayJudgement();
                break;
            case EInterviewState.SubjectEntry:
                PlayIntro(currentSubject);
                break;
            case EInterviewState.AwaitingSentenceInput:
                PlayAwaitSentence();
                break;
            case EInterviewState.NoSubject:
                PlayNoSubject();
                break;
            case EInterviewState.Confession:
                PlayConfession();
                break;
        }
    }

    private void PlayConfession()
    {
        Debug.Assert(currentSubject != null);
        PlayAnimation(ESubjectClipType.Confession, false);
    }

    private void PlayNoSubject()
    {
        currentClipData = null;
        currentSubject = null;
        videoPlayer.Stop();
        videoPlayer.clip = null;
       // videoPlayer.enabled = false;
    }

    private void PlayAwaitSentence()
    {
        Debug.Assert(currentSubject != null);
        PlayAnimation(ESubjectClipType.AwaitSentencing, true);
    }

    private void PlayIntro(SubjectInformation newSubject)
    {
        Debug.Assert(newSubject != null);
        currentSubject = newSubject;
        PlayAnimation(ESubjectClipType.Entry, false);
    }

    private void PlayJudgement()
    {
        Debug.Assert(currentSubject != null);
        PlayAnimation(ESubjectClipType.Sentenced, false);
    }

    private void PlayAnimation(ESubjectClipType clip, bool loop)
    {
        videoPlayer.Stop();
        currentClipData = currentSubject.GetClipData(clip);
        videoPlayer.isLooping = loop;
        vcap.SetVideoURL(currentClipData.videoClipURL);
        videoPlayer.Prepare();
    }

    private void NotifyVideoPrepareCompleted(VideoPlayer source)
    {
        videoPlayer.Play();
    }

    private void NotifyClipLoopPointReached(VideoPlayer source)
    {
        onSubjectClipCompleted?.Invoke(currentClipData, videoPlayer.isLooping);
    }
}