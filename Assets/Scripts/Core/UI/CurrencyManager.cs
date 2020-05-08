using System;
using Core.Managers;
using Core.UI.Prefabs;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class CurrencyManager : GlobalClass
    {
        public Currency[] currencies;
        public RectTransform currenciesRect;

        [Header("CurrencyUpdate")] [SerializeField]
        private AddCurrencyPrefab addCurrencyPrefab;

        [SerializeField] private TweenSetting addCurrencyTweenSetting;
        public Transform addCurrencySpawnPosition;

        private MyObjectPool<AddCurrencyPrefab> _tweenCurrencyPool;
        private Vector2 _newSizeDelta = new Vector2();
        private bool _updateCurrencies;

        [ContextMenu("addCurrencyTest")]
        public void TestAddCurrency()
        {
            PlayerData.wallet[0].resourceAmount += 20;

            RefreshCurrency(currencies[0]);
        }

        private void Start()
        {
            _tweenCurrencyPool =
                ObjectPoolManager.CreateObjectPool<AddCurrencyPrefab>(addCurrencyPrefab, addCurrencySpawnPosition);

            SetupCurrencies();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshAllCurrencies);
        }

        private void SetupCurrencies()
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                currencies[i].currencyDetails.resourceAmount =
                    PlayerData.GetResourceAmount(currencies[i].currencyDetails.resourceType);

                currencies[i].UpdateTextDisplay();
            }

            ResizeCurrencyRect();
        }

        private void Update()
        {
            if (!_updateCurrencies) return;

            UpdateCurrencies();

            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                if (!currencies[i].updateCurrency) continue;

                _updateCurrencies = true;
            }
        }

        private void RefreshAllCurrencies()
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                RefreshCurrency(currencies[i]);
            }
        }

        private void RefreshCurrency(Currency currency)
        {
            var currencyDifference = PlayerData.GetResourceAmount(currency.currencyDetails.resourceType) -
                                     currency.currencyDetails.resourceAmount;

            if (currencyDifference == 0) return;

            currency.currencyDifference = currencyDifference;

            if (currencyDifference > 0)
                AddCurrency(currency);
            else
                DeductCurrency(currency);
        }

        private void UpdateCurrencies()
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                if (!currencies[i].updateCurrency) continue;

                currencies[i].UpdateTextDisplay();
            }
            
            _updateCurrencies = false;
        }

        private void AddCurrency(Currency currency)
        {
            var currencyInstanceAmount = addCurrencyTweenSetting.GetSpawnAmount(currency.currencyDifference);
            ResizeCurrencyRect();

            var sequence = DOTween.Sequence();
            for (var i = 0; i < currencyInstanceAmount; i++)
            {
                var currencyInstance = _tweenCurrencyPool.Get();
                currencyInstance.Init(currency.currencyIcon.sprite);
                currencyInstance.gameObject.SetActive(false);
                currencyInstance.transform.Rotate(Vector3.forward, Random.Range(-150.0f, -10.0f));
                currencyInstance.transform.localPosition = addCurrencyTweenSetting.RandomSpawnPosition();
                sequence.InsertCallback(i * addCurrencyTweenSetting.delayBetweenInstance,
                        () => currencyInstance.gameObject.SetActive(true))
                    .Insert(i * addCurrencyTweenSetting.delayBetweenInstance,
                        currencyInstance.transform.DOMove(currency.currencyIcon.transform.position,
                                addCurrencyTweenSetting.moveDuration)
                            .SetEase(addCurrencyTweenSetting.moveCurve)
                            .OnComplete(() =>
                            {
                                _tweenCurrencyPool.Release(currencyInstance);
                                currencyInstance.gameObject.SetActive(false);
                                currencyInstance.transform.localPosition = Vector3.zero;
                                currency.DoPunch();
                            })).SetRecyclable(false);
            }

            var addingCurrencyDuration = sequence.Duration(false) - addCurrencyTweenSetting.moveDuration;

            var currencyRef = currency; //isolation

            sequence.Insert(addCurrencyTweenSetting.moveDuration,
                    DOTween.To(() => currencyRef.currencyDetails.resourceAmount,
                        x => currencyRef.currencyDetails.resourceAmount = x,
                        PlayerData.GetResourceAmount(currency.currencyDetails.resourceType), addingCurrencyDuration))
                .OnComplete(() =>
                {
                    currencyRef.UpdateTextDisplay();
                    currencyRef.updateCurrency = false;
                });

            currency.updateCurrency = true;
            _updateCurrencies = true;
        }

        private static void DeductCurrency(Currency currency)
        {
            currency.deductCurrencyPrefab.Init(currency.currencyDifference);
            currency.currencyDetails.resourceAmount =
                PlayerData.GetResourceAmount(currency.currencyDetails.resourceType);
            currency.UpdateTextDisplay();
        }

        public Currency GetCurrencyByType(ResourceType currencyType)
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
                if (currencies[i].currencyDetails.resourceType == currencyType)
                    return currencies[i];

            return new Currency();
        }

        public void ResizeCurrencyRect()
        {
            long highestAmount = 0;

            var walletLength = PlayerData.wallet.Length;
            for (var i = 0; i < walletLength; i++)
                if (PlayerData.wallet[i].resourceAmount > highestAmount)
                    highestAmount = PlayerData.wallet[i].resourceAmount;

            var digitCount = Math.Floor(Math.Log10(Mathf.Abs(highestAmount)) + 1);
            _newSizeDelta.Set(double.IsInfinity(digitCount)
                ? Constants.CoinsBackgroundBaseWidth + Constants.CoinsBackgroundWidthMultiplier
                : (float) digitCount * Constants.CoinsBackgroundWidthMultiplier +
                  Constants.CoinsBackgroundBaseWidth, currenciesRect.sizeDelta.y);

            currenciesRect.DOSizeDelta(_newSizeDelta, addCurrencyTweenSetting.sizeDeltaDuration);
        }
    }
}