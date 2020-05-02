using System;
using Core.Managers;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class CurrencyController : GlobalClass
    {
        public struct CurrencyStruct
        {
            // public long currencyAmount { get; private set; }
            public Transform currencyIcon;
            public TMP_Text currencyAmountText;
            public RectTransform currencyRect;
            public Currency currencyDetails;
            public DeductCurrencyPrefab deductCurrencyPrefab;
            public bool updateCurrency;

            public void UpdateTextDisplay()
            {
                currencyAmountText.text = currencyDetails.currencyAmount.ToString();
                updateCurrency = false;
            }
        }

        public CurrencyStruct[] currencies;
        private Transform currencyPrefab;
        public Transform currencySpawnPosition;

        // [SerializeField] private TMP_Text bananaCoinsAmountText;
        // [SerializeField] private TMP_Text bluePrintsAmountText;
        // [SerializeField] private TMP_Text starFruitsAmountText;
        // public long bcAmount { get; private set; }
        // public long bpAmount { get; private set; }
        // public long sfAmount { get; private set; }

        [Header("CurrencyUpdate")]
        // [SerializeField] private Transform currencyPrefab;

        // [SerializeField] private Transform currencyIcon;
        // [SerializeField] private Transform currencySpawnPosition;
        [SerializeField] private TweenSetting addCurrencyTweenSetting;
        private MyObjectPool<Transform> _tweenCurrencyPool;

        // private RectTransform _bananaCoinsCurrencyRect;
        // private RectTransform _bluePrintsCurrencyRect;
        // private RectTransform _starFruitsCurrencyRect;

        private long _currencyDifference;
        private bool _updatingCurrencies;
        private CurrencyType _updatingCurrencyType;
        public bool withoutTweens;

        private void Start()
        {
            _tweenCurrencyPool = ObjectPoolManager.CreateObjectPool<Transform>(currencyPrefab, currencySpawnPosition);

            //create for loop of currencies to init
            SetupCurrencies();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshAllCurrencies);
        }

        private void SetupCurrencies()
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                currencies[i].currencyRect = (RectTransform) currencies[i].currencyAmountText.transform.parent.parent
                    .GetComponent(typeof(RectTransform));
                currencies[i].currencyDetails.currencyAmount =
                    PlayerData.GetResourceAmount(currencies[i].currencyDetails.currencyType);
                
                //add to general refresh
                currencies[i].UpdateTextDisplay();
                ResizeCurrencySizeDelta(currencies[i].currencyDetails.currencyType);
            }

            withoutTweens = true;
        }

        private void Update()
        {
            if (!_updatingCurrencies) return;

            UpdateCurrencies();
        }

        private void RefreshAllCurrencies()
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                RefreshCurrency(currencies[i]);
            }

            withoutTweens = false;
        }

        private void RefreshCurrency(CurrencyStruct currency)
        {
            _currencyDifference = PlayerData.GetResourceAmount(currency.currencyDetails.currencyType) -
                                  currency.currencyDetails.currencyAmount;

            if (_currencyDifference == 0) return;

            if (_currencyDifference > 0)
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
                
                GetCurrencyByType(_updatingCurrencyType).UpdateTextDisplay();
            }
        }

        //resize the width of the coin currency background according to the number of digits 
        public void ResizeCurrencySizeDelta(CurrencyType currencyType)
        {
            var currency = GetCurrencyByType(currencyType);
            
            var newSizeDelta =
                ResizeCurrencyRectByDigitCount(currency.currencyRect,
                    PlayerData.GetResourceAmount(CurrencyType.BananaCoins));
            currency.currencyRect.DOSizeDelta(newSizeDelta, addCurrencyTweenSetting.sizeDeltaDuration);
        }

        private static Vector2 ResizeCurrencyRectByDigitCount(RectTransform rect, long amount)
        {
            var digitCount = Math.Floor(Math.Log10(Mathf.Abs(amount)) + 1);
            return new Vector2(double.IsInfinity(digitCount)
                ? Constants.CoinsBackgroundBaseWidth + Constants.CoinsBackgroundWidthMultiplier
                : (float) digitCount * Constants.CoinsBackgroundWidthMultiplier +
                  Constants.CoinsBackgroundBaseWidth, rect.sizeDelta.y);
        }

        private void AddCurrency(CurrencyStruct currency)
        {
            //get a bunch of coins from the pool and move them to the currency target
            var currencyInstanceAmount = TweenSetting.GetSpawnAmount(_currencyDifference);
            ResizeCurrencySizeDelta(currency.currencyDetails.currencyType);

            var sequence = DOTween.Sequence();
            for (var i = 0; i < currencyInstanceAmount; i++)
            {
                var currencyInstance = _tweenCurrencyPool.Get();
                currencyInstance.gameObject.SetActive(false);
                currencyInstance.transform.Rotate(Vector3.forward, Random.Range(-150.0f, -10.0f));
                currencyInstance.transform.localPosition = addCurrencyTweenSetting.RandomSpawnPosition();
                sequence.InsertCallback(i * addCurrencyTweenSetting.delayBetweenInstance,
                        () => currencyInstance.gameObject.SetActive(true))
                    .Insert(i * addCurrencyTweenSetting.delayBetweenInstance,
                        currencyInstance.DOMove(currency.currencyIcon.position,
                                addCurrencyTweenSetting.moveDuration)
                            .SetEase(addCurrencyTweenSetting.moveCurve)
                            .OnComplete(() =>
                            {
                                _tweenCurrencyPool.Release(currencyInstance);
                                currencyInstance.gameObject.SetActive(false);
                                currencyInstance.transform.localPosition = Vector3.zero;
                                addCurrencyTweenSetting.DoPunch(currency.currencyIcon.transform);
                            })).SetRecyclable(true);
            }

            //increment the coinsAmountText to the new amount
            var addingCurrencyDuration = sequence.Duration(false) - addCurrencyTweenSetting.moveDuration;
            sequence.Insert(addCurrencyTweenSetting.moveDuration,
                DOTween.To(() => currency.currencyDetails.currencyAmount, x => currency.currencyDetails.currencyAmount = x,
                    PlayerData.GetResourceAmount(CurrencyType.BananaCoins),
                    addingCurrencyDuration).OnComplete(() =>
                {
                    _updatingCurrencies = false;
                    currency.currencyAmountText.text = currency.currencyDetails.currencyAmount.ToString();
                }));

            currency.updateCurrency = true;
            _updatingCurrencies = true;
        }

        private void DeductCurrency(CurrencyStruct currency)
        {
            //instantiate a duplicate of the currency display (amount and icon) and tween it
            currency.deductCurrencyPrefab.Init(currency.currencyDetails);
            //update the currency display to show the new amount
            currency.currencyDetails.currencyAmount = PlayerData.GetResourceAmount(currency.currencyDetails.currencyType);
            currency.currencyAmountText.text = currency.currencyDetails.currencyAmount.ToString();
        }

        public CurrencyStruct GetCurrencyByType(CurrencyType currencyType)
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
                if (currencies[i].currencyDetails.currencyType == currencyType)
                    return currencies[i];

            return new CurrencyStruct();
        }
    }
}