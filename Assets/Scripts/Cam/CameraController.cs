using System;
using System.Collections;
using DG.Tweening;
using Extensions;
using General;
using UnityEngine;

namespace Cam
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector2 initialPosition;
        [SerializeField] private float initialSize = 5;

        [Space] [SerializeField] private Vector2 focusPosition;
        [SerializeField] private float focusSize = 5;

        [Space] [SerializeField] private float posDuration = 0.7f;
        [SerializeField] private float sizeDuration = 0.7f;

        [SerializeField] private float zoomInDelay = 0.7f;


        [Space] [SerializeField] [ReadOnlyField]
        private bool isInFocus;

        [SerializeField] [ReadOnlyField] private bool isTransitioning;

        private void Awake()
        {
            Hub.Register(this);
        }

        private void Start()
        {
            var inter = Hub.Get<Interview>();
            if (inter)
                inter.onInterviewProgression += OnInterviewProgressed;
        }


        private void OnInterviewProgressed(SubjectInformation currentsubject, EInterviewState interviewstate)
        {
            switch (interviewstate)
            {
                case EInterviewState.AwaitingSentenceInput:
                    SetFocus(true, zoomInDelay);
                    break;
                default:
                    SetFocus(false);
                    break;
            }
        }

        public void SetFocus(bool focus, float delay = 0)
        {
            if (focus == isInFocus)
                return;

            StartCoroutine(SetFocusCoroutine(focus, delay));
        }

        private IEnumerator SetFocusCoroutine(bool focus, float delay = 0)
        {
            isTransitioning = true;
            isInFocus = focus;

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            var targetPos = (Vector3) (focus ? focusPosition : initialPosition);
            targetPos.z = -10;

            var targetSize = focus ? focusSize : initialSize;

            transform.DOMove(targetPos, posDuration);
            Camera.main.DOOrthoSize(targetSize, sizeDuration);

            yield return new WaitForSeconds(Math.Max(posDuration, sizeDuration));

            isTransitioning = false;
        }

#if UNITY_EDITOR

        [UnityEditor.CustomEditor(typeof(CameraController))]
        public class CamControllerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var camController = target as CameraController;
                if (camController == null)
                {
                    return;
                }

                if (Application.isPlaying && GUILayout.Button("Toggle Position") && !camController.isTransitioning)
                {
                    camController.SetFocus(!camController.isInFocus);
                }
            }
        }

#endif
    }
}