using Enums;
using UnityEngine;

namespace Core.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] public bool soundsDisabled;
        [SerializeField] public bool musicDisabled;
        
        /*protected AudioManager()
        {
        }*/

        private AudioSource _audioSource;

        private void OnEnable()
        {
            if (CameraManager == null) return;
            
            _audioSource = (AudioSource) CameraManager.MainCamera.GetComponent(typeof(AudioSource));
        }

        public void PlayClip(SoundEffectType soundEffectType)
        {
            if (soundsDisabled) return;
            
            var soundEffect = ResourceManager.GetSoundEffect(soundEffectType);
            if (soundEffect == null) return;

            _audioSource.clip = soundEffect;
            _audioSource.Play();
        }
    }
}