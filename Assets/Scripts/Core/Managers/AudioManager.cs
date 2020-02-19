using System;
using Enums;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected AudioManager()
        {
        }

        private AudioSource _audioSource;
        private MusicTrack[] _musicTracks;
        private SoundEffect[] _soundClips;

        private void OnEnable()
        {
            _audioSource = (AudioSource) CameraManager.MainCamera.GetComponent(typeof(AudioSource));
            _musicTracks = Resources.LoadAll<MusicTrack>(Constants.MusicTrackPath);
            _soundClips = Resources.LoadAll<SoundEffect>(Constants.SoundEffectPath);
        }

        public void PlayClip(SoundEffectType soundEffectType)
        {
            if (_soundClips.Length < 1) return;

            foreach (var soundClip in _soundClips)
            {
                if (soundClip.soundEffectType == soundEffectType && soundClip.soundEffectArray.Length > 0)
                    _audioSource.clip = soundClip.soundEffectArray[Random.Range(0, soundClip.soundEffectArray.Length)];
            }
        }
    }
}