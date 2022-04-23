using System;
using Extensions;
using UnityEngine;
using UnityEngine.Video;

namespace Videos
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoClipAssetPlayer : MonoBehaviour
    {
        [SerializeField] private string videoName;
        [SerializeField] [ReadOnlyField] private bool isValid;


        private void Awake()
        {
            SetVideoURL();
        }

        private void OnValidate()
        {
            SetVideoURL();
        }

        private void SetVideoURL()
        {
            var videoPlayer = GetComponent<VideoPlayer>();

            if (!videoPlayer || videoPlayer.clip || string.IsNullOrWhiteSpace(videoName))
                return;

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        }
    }
}