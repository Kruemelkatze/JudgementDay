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
            SetVideoURL(videoName);
        }

        private void OnValidate()
        {
            //SetVideoURL(videoName);
        }

        public void SetVideoURL(string filename)
        {
            var videoPlayer = GetComponent<VideoPlayer>();

            if (!videoPlayer ||  string.IsNullOrWhiteSpace(filename))
                return;
            if(videoPlayer.clip)
            {
                Debug.LogError("videoPlayer already has clip");
                return;
            }

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
        }
    }
}