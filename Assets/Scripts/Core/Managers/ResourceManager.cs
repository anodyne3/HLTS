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
        private readonly Dictionary<string, Sprite> fruitParticleSprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, MusicTrack> musicTracks = new Dictionary<string, MusicTrack>();

        private SoundEffect[] _soundEffects;
        private MusicTrack[] _musicTracks;

        private readonly List<IMyDictionaries> AllDictionaries = new List<IMyDictionaries>();

        private interface IMyDictionaries
        {
        }

        private class DictionaryReference<T> : IMyDictionaries
        {
            public readonly IDictionary<string, T> reference;
            public Type Type => typeof(T);
            private T Value;

            public DictionaryReference(IDictionary<string, T> dictionary)
            {
                reference = dictionary;
            }
        }

        private void Awake()
        {
            PopulateDictionary(fruitParticleSprites, Constants.FruitParticleSpritesPath);
            PopulateDictionary(musicTracks, Constants.MusicTrackPath);

            _soundEffects = Resources.LoadAll<SoundEffect>(Constants.SoundEffectPath);
            _musicTracks = Resources.LoadAll<MusicTrack>(Constants.MusicTrackPath);
        }

        //load all resources from a path in the Resources folder, and add them to a dictionary
        private void PopulateDictionary<T>(IDictionary<string, T> resourceDictionary, string path) where T : Object
        {
            var resources = Resources.LoadAll<T>(path);

            var resourcesLength = resources.Length;
            for (var i = 0; i < resourcesLength; i++)
            {
                resourceDictionary.Add(resources[i].name, resources[i]);
            }

            var newDictionaryReference = new DictionaryReference<T>(resourceDictionary);
            AllDictionaries.Add(newDictionaryReference);
        }

        //return an object from it's equivalent dictionary
        private T GetResource<T>(string keyString) where T : Object
        {
            var AllDictionariesCount = AllDictionaries.Count;
            var dictionaryReference = new DictionaryReference<T>(null);
            for (var i = 0; i < AllDictionariesCount; i++)
            {
                dictionaryReference = AllDictionaries[i] as DictionaryReference<T>;
                if (dictionaryReference == null || dictionaryReference.Type != typeof(T)) continue;

                if (dictionaryReference.reference.TryGetValue(keyString, out var value))
                    return value;
            }

            if (dictionaryReference != null)
                Debug.LogError(keyString + " resource missing from " + dictionaryReference.Type);
            return null;
        }

        //return a Sprite from its dictionary according to it's FruitType
        public Sprite GetFruitParticleSprite(FruitType fruitType)
        {
            if (fruitType != FruitType.Barnana)
                return GetResource<Sprite>(fruitType + "Sprite");

            var randomSprite = Random.Range(1, 11);

            return randomSprite % 2 == 1
                ? GetResource<Sprite>(FruitType.Bars + "Sprite")
                : GetResource<Sprite>(FruitType.Bananas + "Sprite");
        }

        //return a random AudioClip from a SoundEffect scriptableObject by its enum 
        public AudioClip GetSoundEffect(SoundEffectType soundEffectType)
        {
            if (_soundEffects.Length < 1)
            {
                // Debug.LogError("empty soundEffect");
                return null;
            }

            var _soundEffectsLength = _soundEffects.Length;
            for (var i = 0; i < _soundEffectsLength; i++)
            {
                var soundClip = _soundEffects[i];
                if (soundClip.soundEffectType == soundEffectType && soundClip.soundEffectArray.Length > 0)
                    return soundClip.soundEffectArray[Random.Range(0, soundClip.soundEffectArray.Length)];
            }

            // Debug.LogError("empty soundEffectArray");
            return null;
        }

        //return an audioClip from a MusicTrack scriptableObject by its enum
        public AudioClip GetMusicTrack(MusicStyle musicStyle)
        {
            return GetResource<MusicTrack>(musicStyle.ToString()).musicTrack;
        }
    }
}