using System.Collections.Generic;
using Core.MainSlotMachine;
using Enums;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField] private Dictionary<string, FruitParticle> fruitParticlePrefabs = new Dictionary<string, FruitParticle>();

        private void Awake()
        {
            PopulateDictionary(fruitParticlePrefabs, Constants.FruitParticlePrefabsPath);
        }

        private static void PopulateDictionary<T>(IDictionary<string, T> resourceDictionary, string path)
            where T : Object
        {
            var resources = Resources.LoadAll<T>(path);

            var resourcesLength = resources.Length;
            for (var i = 0; i < resourcesLength; i++)
            {
                resourceDictionary.Add(resources[i].name, resources[i]);
            }
        }

        public FruitParticle GetFruitParticlePrefab(FruitType fruitType)
        {
            if (fruitType != FruitType.Barnana)
            {
                if (fruitParticlePrefabs.TryGetValue(fruitType.ToString(), out var value))
                    return value;
            }

            var randomSprite = Random.Range(1, 11);
            
            if (randomSprite % 2 == 1)
            {
                if (fruitParticlePrefabs.TryGetValue(FruitType.Bar.ToString(), out var value))
                    return value;
            }
            else
            {
                if (fruitParticlePrefabs.TryGetValue(FruitType.Banana.ToString(), out var value))
                    return value;
            }

            return null;
        }
    }
}