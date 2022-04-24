using System;
using System.Collections;
using Cam;
using UnityEngine;
using UnityEngine.Video;

namespace General
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private string nextScene = "2DScene";
        [SerializeField] private VideoPlayer player;

        [SerializeField] private float zoomDelay = 29;

        private void Start()
        {
            StartCoroutine(ZoomAfterVideo());
        }

        private IEnumerator ZoomAfterVideo()
        {
            yield return new WaitForSeconds(zoomDelay);
            Hub.Get<CameraController>().SetFocus(true);
        }

        public void Play()
        {
            SceneController.Instance.LoadScene(nextScene);
        }
    }
}