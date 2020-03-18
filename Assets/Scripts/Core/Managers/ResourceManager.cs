using System.Collections.Generic;
using System.Linq;
using Core.MainSlotMachine;
using Enums;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField] private Dictionary<string, FruitParticle> fruitSprites = new Dictionary<string, FruitParticle>();

        private void Awake()
        {
            PopulateDictionary(fruitSprites, Constants.FruitSpritesPath);
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

        public FruitParticle GetFruitSprite(FruitType fruitType)
        {
            if (fruitType != FruitType.Barnana)
                return fruitSprites.Where(x => x.Key == fruitType.ToString())
                    .Select(x => x.Value).FirstOrDefault();

            var randomSprite = Random.Range(1, 11);
            return randomSprite % 2 == 1
                ? fruitSprites.First(x => x.Key == FruitType.Bar.ToString()).Value
                : fruitSprites.First(x => x.Key == FruitType.Banana.ToString()).Value;
        }
    }
}