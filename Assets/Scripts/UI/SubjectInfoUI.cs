using System;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SubjectInfoUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Color highlightColor = Color.green;

        [SerializeField] [ReadOnlyField] public SubjectInformation subjectInfo;
        [SerializeField] [ReadOnlyField] public bool highlighted;

        public event Action<SubjectInfoUI> OnClicked;

        public void SetSubjectInfo(SubjectInformation info)
        {
            subjectInfo = info;
            image.sprite = subjectInfo.sprite;
            nameField.text = subjectInfo.subjectName;

            SetHighlighted(false);
        }

        public void SetHighlighted(bool high)
        {
            highlighted = high;

            highlightImage.color = high ? highlightColor : Color.white;
        }

        public void Clicked()
        {
            AudioController.Instance.PlaySoundFromUI("click");
            OnClicked?.Invoke(this);
        }
    }
}