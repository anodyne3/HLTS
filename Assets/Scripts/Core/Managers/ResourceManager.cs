using System;
using System.Collections.Generic;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private readonly Dictionary<string, Sprite> _fruitParticleSprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, MusicTrack> _musicTracks = new Dictionary<string, MusicTrack>();
        private readonly Dictionary<string, Sprite> _currencySprites = new Dictionary<string, Sprite>();

        private SoundEffect[] _soundEffects;

        private readonly List<IMyDictionaries> _allDictionaries = new List<IMyDictionaries>();

        private interface IMyDictionaries
        {
        }

        private class DictionaryReference<T> : IMyDictionaries
        {
            public readonly IDictionary<string, T> reference;
            public Type Type => typeof(T);
            // private T Value;

            public DictionaryReference(IDictionary<string, T> dictionary)
            {
                reference = dictionary;
            }
        }

        private void Awake()
        {
            PopulateDictionary(_fruitParticleSprites, Constants.FruitParticleSpritesPath);
            PopulateDictionary(_musicTracks, Constants.MusicTrackPath);
            PopulateDictionary(_currencySprites, Constants.ChestRewardsPath);

            _soundEffects = Resources.LoadAll<SoundEffect>(Constants.SoundEffectPath);
        }

        private void PopulateDictionary<T>(IDictionary<string, T> resourceDictionary, string path) where T : Object
        {
            var resources = Resources.LoadAll<T>(path);

            var resourcesLength = resources.Length;
            for (var i = 0; i < resourcesLength; i++)
            {
                resourceDictionary.Add(resources[i].name, resources[i]);
            }

            var newDictionaryReference = new DictionaryReference<T>(resourceDictionary);
            _allDictionaries.Add(newDictionaryReference);
        }

        private T GetResource<T>(string keyString) where T : Object
        {
            var dictionaryReference = new DictionaryReference<T>(null);
            foreach (var dictionary in _allDictionaries)
            {
                dictionaryReference = dictionary as DictionaryReference<T>;
                if (dictionaryReference == null || dictionaryReference.Type != typeof(T)) continue;

                if (dictionaryReference.reference.TryGetValue(keyString, out var value))
                    return value;
            }

            if (dictionaryReference != null)
                Debug.LogError(keyString + " resource missing from " + dictionaryReference.Type);
            return null;
        }

        #region Sprites
        public Sprite GetFruitParticleSprite(FruitType fruitType)
        {
            if (fruitType != FruitType.Barnana)
                return GetResource<Sprite>(fruitType + "Sprite");

            var randomSprite = Random.Range(1, 11);

            return randomSprite % 2 == 1
                ? GetResource<Sprite>(FruitType.Bars + "Sprite")
                : GetResource<Sprite>(FruitType.Bananas + "Sprite");
        }

        public Sprite GetCurrencySprite(ResourceType currencyType)
        {
            return GetResource<Sprite>(currencyType + "Sprite");
        }
        #endregion
        
        #region AudioClips
        public AudioClip GetSoundEffect(SoundEffectType soundEffectType)
        {
            if (_soundEffects.Length < 1)
            {
                return null;
            }

            var soundEffectsLength = _soundEffects.Length;
            for (var i = 0; i < soundEffectsLength; i++)
            {
                var soundClip = _soundEffects[i];
                if (soundClip.soundEffectType == soundEffectType && soundClip.soundEffectArray.Length > 0)
                    return soundClip.soundEffectArray[Random.Range(0, soundClip.soundEffectArray.Length)];
            }

            return null;
        }

        public AudioClip GetMusicTrack(MusicStyle musicStyle)
        {
            return GetResource<MusicTrack>(musicStyle.ToString()).musicTrack;
        }
        #endregion
    }
}
