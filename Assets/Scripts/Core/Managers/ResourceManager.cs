using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField] private Dictionary<string, Sprite> fruitParticleSprites = new Dictionary<string, Sprite>();

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
            for (var i = 0; i < AllDictionariesCount; i++)
            {
                var dictionaryReference = AllDictionaries[i] as DictionaryReference<T>;
                if (dictionaryReference == null || dictionaryReference.Type != typeof(T)) continue;

                if (dictionaryReference.reference.TryGetValue(keyString, out var value))
                    return value;
            }

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
    }
}