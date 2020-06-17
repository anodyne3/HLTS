using System.Collections;
using Core.UI;
using Enums;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.MainSlotMachine
{
    public class CoinGenerator : GlobalAccess
    {
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
            var waitForSeconds = new WaitForSeconds(0.1f);

            while (CoinTray.CoinTrayCounter < Constants.CoinTrayMax)
            {
                var nextCoin = (CoinDragHandler) ObjectPoolManager.coinPool.Get().GetComponent(typeof(CoinDragHandler));
                nextCoin.transform.position = GeneratedSpawnPosition();
                ResetCoin(nextCoin);
                EventManager.coinCreated.Raise();
                yield return waitForSeconds;
            }
        }

        private static Vector2 GeneratedSpawnPosition()
        {
            return new Vector2(Random.Range(-Constants.SpawnRange, Constants.SpawnRange), Constants.SpawnHeight);
        }

        private static void ResetCoin(CoinDragHandler nextCoin)
        {
            nextCoin.CircleCollider.enabled = true;
            nextCoin.SetCoinOrderInLayer(Constants.CoinStartingSortingOrder);
            nextCoin.gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(typeof(CoinDragHandler), out var droppedObject)) return;

            droppedObject.gameObject.SetActive(false);
            var droppedCoinDragHandler = (CoinDragHandler) droppedObject;
            droppedCoinDragHandler.CircleCollider.isTrigger = false;
            ObjectPoolManager.coinPool.Release((CoinDragHandler) droppedObject);
            EventManager.coinDropped.Raise();
        }
    }
}