using System.Collections;
using Core.UI;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    public class CoinGenerator : GlobalAccess
    {
        //to const
        [SerializeField] private int coinTrayMax = 12;
        [SerializeField] private CoinDragHandler coinPrefab;

        private void Start()
        {
            ObjectPoolManager.coinPool = new MyObjectPool<CoinDragHandler>(() => Instantiate(coinPrefab, transform)); 
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.generateCoinEvent, UpdateCoins);
            UpdateCoins();
        }

        private void UpdateCoins()
        {
            StartCoroutine(nameof(CreateCoin));
        }

        private IEnumerator CreateCoin()
        {
            while (CoinTray.CoinTrayCounter < PlayerData.coinsAmount && CoinTray.CoinTrayCounter < coinTrayMax)
            {
                var nextCoin = ObjectPoolManager.coinPool.Get();
                nextCoin.transform.position = GeneratedSpawnPosition();
                ResetCoin(nextCoin);
                EventManager.coinCreated.Raise();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static Vector2 GeneratedSpawnPosition()
        {
            return new Vector2(Random.Range(-Constants.SpawnRange, Constants.SpawnRange), Constants.SpawnHeight);
        }

        private static void ResetCoin(CoinDragHandler nextCoin)
        {
            nextCoin.CircleCollider.enabled = true;
            nextCoin.SetCoinOrderInLayer(0);
            nextCoin.gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(typeof(CoinDragHandler), out var droppedObject)) return;
            
            droppedObject.gameObject.SetActive(false);
            ObjectPoolManager.coinPool.Release((CoinDragHandler) droppedObject);
            EventManager.coinDropped.Raise();
        }
    }
}