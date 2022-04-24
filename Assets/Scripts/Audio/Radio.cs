using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Videos;
using Random = UnityEngine.Random;

namespace General
{
    public class Radio : MonoBehaviour
    {
        [SerializeField] private string radioKnobSound = "radioknob";
        [SerializeField] private float switchDelay = 0.7f;


        private int currentIndex;

        private string[] songs;
        private Coroutine _switchCo;

        private void Start()
        {
            songs = AudioController.Instance.musicClips.Keys.ToArray();
            var index = (int) (Random.value * songs.Length);
            PlaySong(index, true);
        }

        public void NextSong()
        {
            var index = (currentIndex + 1) % songs.Length;
            PlaySong(index);
        }

        public void PlaySong(int index, bool immediate = false)
        {
            if (songs.Length == 0)
                return;

            index = Math.Clamp(index, 0, songs.Length - 1);

            if (_switchCo != null)
            {
                StopCoroutine(_switchCo);
            }

            _switchCo = StartCoroutine(PlaySongCoroutine(index, immediate));
        }

        private IEnumerator PlaySongCoroutine(int index, bool immediate)
        {
            if (!immediate)
            {
                var vidc = Hub.Get<TableVideoController>();
                if (vidc)
                {
                    vidc.SetRadioVideo();
                }

                yield return new WaitForSeconds(switchDelay);
            }

            currentIndex = index;

            var song = songs[index];
            AudioController.Instance.PlayMusic(song);

            if (!immediate)
            {
                AudioController.Instance.PlaySound(radioKnobSound);
            }

            _switchCo = null;
        }
    }
}