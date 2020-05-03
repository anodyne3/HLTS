using System;
using System.Linq;
using Core.Managers;
using Core.UI.Prefabs;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class CurrencyController : GlobalClass
    {
        [Serializable]
        public class CurrencyClass
        {
            public Image currencyIcon;
            public TMP_Text currencyAmountText;
            public TweenPunchSetting addCurrencyPunchTween;
            public Currency currencyDetails;
            public DeductCurrencyPrefab deductCurrencyPrefab;
            [HideInInspector] public int currencyDifference;
            [HideInInspector] public bool updateCurrency;

            public void UpdateTextDisplay()
            {
                currencyAmountText.text = currencyDetails.currencyAmount.ToString();
            }

            public void DoPunch()
            {
                addCurrencyPunchTween.DoPunch(currencyIcon.transform);
            }
        }

        public CurrencyClass[] currencies;
        public RectTransform currenciesRect;

        [Header("CurrencyUpdate")] [SerializeField]
        private AddCurrencyPrefab addCurrencyPrefab;

        [SerializeField] private TweenSetting addCurrencyTweenSetting;
        public Transform addCurrencySpawnPosition;

        private MyObjectPool<AddCurrencyPrefab> _tweenCurrencyPool;
        private Vector2 _newSizeDelta = new Vector2();
        private bool _updateCurrencies;

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
                currencies[i].currencyDetails.currencyAmount =
                    PlayerData.GetResourceAmount(currencies[i].currencyDetails.currencyType);

                currencies[i].UpdateTextDisplay();
            }

            ResizeCurrencySizeDelta();
        }

        private void Update()
        {
            if (!_updateCurrencies) return;

            UpdateCurrencies();

            _updateCurrencies = false;

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

        private void RefreshCurrency(CurrencyClass currency)
        {
            var currencyDifference = PlayerData.GetResourceAmount(currency.currencyDetails.currencyType) -
                                     currency.currencyDetails.currencyAmount;

            if (currencyDifference == 0) return;

            currency.currencyDifference = (int) currencyDifference;

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
        }

        [ContextMenu("addCurrencyTest")]
        public void TestAddCurrency()
        {
            PlayerData.wallet[0].currencyAmount += 20;

            RefreshCurrency(currencies[0]);
        }

        private void AddCurrency(CurrencyClass currency)
        {
            //get a bunch of coins from the pool and move them to the currency target
            var currencyInstanceAmount = addCurrencyTweenSetting.GetSpawnAmount(currency.currencyDifference);
            ResizeCurrencySizeDelta();

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

            //increment the coinsAmountText to the new amount
            var addingCurrencyDuration = sequence.Duration(false) - addCurrencyTweenSetting.moveDuration;

            var currencyRef = currency;

            sequence.Insert(addCurrencyTweenSetting.moveDuration,
                    DOTween.To(() => currencyRef.currencyDetails.currencyAmount,
                        x => currencyRef.currencyDetails.currencyAmount = x,
                        PlayerData.GetResourceAmount(currency.currencyDetails.currencyType), addingCurrencyDuration))
                .OnComplete(() =>
                {
                    currencyRef.UpdateTextDisplay();
                    currencyRef.updateCurrency = false;
                });

            currency.updateCurrency = true;
            _updateCurrencies = true;
        }

        private static void DeductCurrency(CurrencyClass currency)
        {
            //instantiate a duplicate of the currency display (amount and icon) and tween it
            currency.deductCurrencyPrefab.Init(currency.currencyDifference);
            //update the currency display to show the new amount
            currency.currencyDetails.currencyAmount =
                PlayerData.GetResourceAmount(currency.currencyDetails.currencyType);
            currency.UpdateTextDisplay();
        }

        public CurrencyClass GetCurrencyByType(CurrencyType currencyType)
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
                if (currencies[i].currencyDetails.currencyType == currencyType)
                    return currencies[i];

            return new CurrencyClass();
        }

        //resize the width of the coin currency background according to the number of digits 
        public void ResizeCurrencySizeDelta()
        {
            long highestAmount = 0;

            var walletLength = PlayerData.wallet.Length;
            for (var i = 0; i < walletLength; i++)
                if (PlayerData.wallet[i].currencyAmount > highestAmount)
                    highestAmount = PlayerData.wallet[i].currencyAmount;

            var digitCount = Math.Floor(Math.Log10(Mathf.Abs(highestAmount)) + 1);
            _newSizeDelta.Set(double.IsInfinity(digitCount)
                ? Constants.CoinsBackgroundBaseWidth + Constants.CoinsBackgroundWidthMultiplier
                : (float) digitCount * Constants.CoinsBackgroundWidthMultiplier +
                  Constants.CoinsBackgroundBaseWidth, currenciesRect.sizeDelta.y);

            currenciesRect.DOSizeDelta(_newSizeDelta, addCurrencyTweenSetting.sizeDeltaDuration);
        }
    }
}