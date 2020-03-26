using Enums;
using UnityEngine;

namespace Core.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected AudioManager()
        {
        }

        private AudioSource _audioSource;

        private void OnEnable()
        {
            _audioSource = (AudioSource) CameraManager.MainCamera.GetComponent(typeof(AudioSource));
        }

        public void PlayClip(SoundEffectType soundEffectType)
        {
            var soundEffect = ResourceManager.GetSoundEffect(soundEffectType); 
            if (soundEffect == null) return;

            _audioSource.clip = soundEffect;
            _audioSource.Play();
        }
    }
}