using System;
using System.Collections;
using General;
using UnityEngine;

namespace Videos
{
    public class TableVideoController : MonoBehaviour
    {
        [SerializeField] private string typing = "typing.mp4";
        [SerializeField] private string bell = "bell.mp4";
        [SerializeField] private string radio = "radio.mp4";


        [SerializeField] private float bellLength = 2.1f;
        [SerializeField] private float radioLength = 4f;
        private bool _init;

        private void Awake()
        {
            Hub.Register(this);
        }

        private void Start()
        {
            Hub.Get<Interview>().onInterviewProgression += OnInterviewProgressed;
        }

        private void OnInterviewProgressed(SubjectInformation currentsubject, EInterviewState interviewstate)
        {
            if (interviewstate == EInterviewState.ExecutingSentence)
            {
                SetBellVideo();

            } 
        }

        public void SetBellVideo()
        {
            StartCoroutine(SetVideoCo(bell, bellLength));
        }

        public void SetRadioVideo()
        {
            StartCoroutine(SetVideoCo(radio, radioLength));
        }

        private IEnumerator SetVideoCo(string vid, float duration)
        {
            GetComponent<VideoClipAssetPlayer>().SetVideoURL(vid);
            yield return new WaitForSeconds(duration);
            GetComponent<VideoClipAssetPlayer>().SetVideoURL(typing);
        }
    }
}