using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General
{
    public class Radio : MonoBehaviour
    {
        private int currentIndex;

        private string[] songs;
        
        private void Start()
        {
            songs = AudioController.Instance.musicClips.Keys.ToArray();
            var index = (int) (Random.value * songs.Length);
            PlaySong(index);
        }

        public void NextSong()
        {
            var index = (currentIndex + 1) % songs.Length;
            PlaySong(index);
        }

        public void PlaySong(int index)
        {
            if (songs.Length == 0)
                return;

            index = Math.Clamp(index, 0, songs.Length - 1);
            currentIndex = index;

            var song = songs[index];
            AudioController.Instance.PlayMusic(song);
        }
    }
}