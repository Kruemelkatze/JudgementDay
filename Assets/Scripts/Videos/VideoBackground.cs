using General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public delegate void OnSubjectClipCompletedDelegate(SubjectClipData clip, bool isLoopingClip);

[RequireComponent(typeof(VideoPlayer))]
public class VideoBackground : MonoBehaviour
{
    public OnSubjectClipCompletedDelegate onSubjectClipCompleted;

    public VideoPlayer videoPlayer;

    public SubjectInformation currentSubject = null;

    public VideoClip defaultClip = null;
    public SubjectClipData currentClipData;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.prepareCompleted += NotifyVideoPrepareCompleted;
        videoPlayer.loopPointReached += NotifyClipLoopPointReached;
        Hub.Register<VideoBackground>(this);
    }

    public void NotifySetupSubjectData(SubjectInformation subject)
    {
        currentSubject = subject;
        currentClipData = subject.GetClipData(ESubjectClipType.Idle);
        videoPlayer.clip = currentClipData.videoClip;
        videoPlayer.Prepare();
    }

    private void NotifyVideoPrepareCompleted(VideoPlayer source)
    {
        videoPlayer.isLooping = currentClipData.shouldLoop;
        videoPlayer.Play();
    }

    private void NotifyClipLoopPointReached(VideoPlayer source)
    {
        onSubjectClipCompleted?.Invoke(currentClipData, videoPlayer.isLooping);
    }
}