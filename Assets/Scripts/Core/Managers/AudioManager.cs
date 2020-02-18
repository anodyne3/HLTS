using System;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class AudioManager : Singleton
    {
        private AudioSource _audioSource;

        [SerializeField] private MusicTrack[] musicTracks;
        [SerializeField] private SoundEffect[] soundClips;
        
        private void Start()
        {
            _audioSource = (AudioSource) CameraManager.MainCamera.GetComponent(typeof(AudioSource));
        }

        public void PlayClip(SoundEffectType soundEffectType)
        {
            foreach (var soundClip in soundClips)
            {
                if (soundClip.soundEffectType == soundEffectType)
                    _audioSource.clip = soundClip.soundEffectArray[Random.Range(0, soundClip.soundEffectArray.Length)];
            }
        }
    }

    [Serializable]
    public class SoundEffect
    {
        public SoundEffectType soundEffectType;
        public AudioClip[] soundEffectArray;
    }

    [Serializable]
    public class MusicTrack
    {
        public AudioClip musicTracks;
    }
}