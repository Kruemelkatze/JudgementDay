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
    public MeshRenderer meshRenderer;

    public SubjectInformation currentSubject = null;

    public SubjectClipData currentClipData = null;

    public GameObject entrySmokeParticles;
    public GameObject sentenceFireParticles;

    public EInterviewState interviewState = EInterviewState.Setup;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        vcap = GetComponent<VideoClipAssetPlayer>();
        videoPlayer.prepareCompleted += NotifyVideoPrepareCompleted;
        videoPlayer.loopPointReached += NotifyClipLoopPointReached;
        Hub.Register<VideoBackground>(this);
    }

    private void Start()
    {
        Hub.Get<Interview>().onInterviewProgression += NotifyInterviewProgression;
    }

    private void NotifyInterviewProgression(SubjectInformation currentSubject, EInterviewState newState)
    {
        interviewState = newState;
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
            case EInterviewState.InterviewDone:
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
        meshRenderer.enabled = false;
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
        ParticleSystem ps = entrySmokeParticles.GetComponent<ParticleSystem>();
        ps.Play();
    }

    private void PlayJudgement()
    {
        Debug.Assert(currentSubject != null);
        PlayAnimation(ESubjectClipType.Sentenced, false);
    }

    private void PlayAnimation(ESubjectClipType clip, bool loop)
    {
        videoPlayer.playbackSpeed = 1.0f;
        currentClipData = currentSubject.GetClipData(clip);
        videoPlayer.isLooping = loop;
        vcap.SetVideoURL(currentClipData.videoClipURL);
        videoPlayer.Prepare();
    }

    private void NotifyVideoPrepareCompleted(VideoPlayer source)
    {

        if(interviewState == EInterviewState.ExecutingSentence)
        {
            meshRenderer.enabled = true;
            videoPlayer.Play();
            ParticleSystem ps = entrySmokeParticles.GetComponent<ParticleSystem>();
            StartCoroutine(PlayParticlesAfterDelay((float)videoPlayer.length * 0.75f, ps));
        }
        else if(interviewState == EInterviewState.SubjectEntry)
        {
            ParticleSystem ps = entrySmokeParticles.GetComponent<ParticleSystem>();
            ps.Play();
            StartCoroutine(PlayAfterDelay(ps.main.duration));
        }
        else
        {
            meshRenderer.enabled = true;
            videoPlayer.Play();
        }
    }

    IEnumerator PlayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        meshRenderer.enabled = true;
        videoPlayer.Play();
    }

    IEnumerator PlayParticlesAfterDelay(float delay, ParticleSystem ps)
    {
        ps.main.duration = videoPlayer.length;
        yield return new WaitForSeconds(delay);
        ps.Play();
    }

    private void NotifyClipLoopPointReached(VideoPlayer source)
    {
        onSubjectClipCompleted?.Invoke(currentClipData, videoPlayer.isLooping);
    }
}