using System;
using Core.Managers;
using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class HudController : GlobalClass
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private TMP_Text coinsAmountText;
        public long CoinsAmount { get; private set; }

        [Header("CurrencyUpdate")]

        [SerializeField] private Transform currencyPrefab;
        [SerializeField] private Transform currencyIcon;
        [SerializeField] private Transform currencySpawnPosition;
        [SerializeField] private TweenSetting addCurrencyTweenSetting;
        [SerializeField] private DeductCurrencyPrefab deductCurrencyPrefab;
        
        private MyObjectPool<Transform> _tweenCurrencyPool;
        private Transform _deductTweenPrefab;
        private RectTransform _currencyRect;
        private long _currencyDifference;
        private bool _addingCurrency;

        private void Start()
        {
            _tweenCurrencyPool = ObjectPoolManager.CreateObjectPool<Transform>(currencyPrefab, currencySpawnPosition);
            
            _currencyRect = (RectTransform) coinsAmountText.transform.parent.parent.GetComponent(typeof(RectTransform));

            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);

            CoinsAmount = PlayerData.coinsAmount;
            coinsAmountText.text = CoinsAmount.ToString();
            ResizeCurrencySizeDelta();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshCoins);
        }

        private void Update()
        {
            if (!_addingCurrency) return;
            
            coinsAmountText.text = CoinsAmount.ToString();
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private void RefreshCoins()
        {
            _currencyDifference = PlayerData.coinsAmount - CoinsAmount;

            if (_currencyDifference == 0) return;

            if (_currencyDifference > 0)
                AddCurrency();
            else
                DeductCurrency();
        }

        //resize the width of the coin currency background according to the number of digits 
        public void ResizeCurrencySizeDelta()
        {
            var newSizeDelta = ResizeCurrencyRectByDigitCount(_currencyRect, PlayerData.coinsAmount);
            _currencyRect.DOSizeDelta(newSizeDelta, addCurrencyTweenSetting.sizeDeltaDuration);
        }

        private static Vector2 ResizeCurrencyRectByDigitCount(RectTransform rect, long amount)
        {
            var digitCount = Math.Floor(Math.Log10(amount) + 1);
            return new Vector2(double.IsInfinity(digitCount)
                ? Constants.CoinsBackgroundBaseWidth + Constants.CoinsBackgroundWidthMultiplier
                : (float) digitCount * Constants.CoinsBackgroundWidthMultiplier +
                  Constants.CoinsBackgroundBaseWidth, rect.sizeDelta.y);
        }

        private void AddCurrency()
        {
            //get a bunch of coins from the pool and move them to the currency target
            var currencyInstanceAmount = TweenSetting.GetSpawnAmount(_currencyDifference);
            ResizeCurrencySizeDelta();
            
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
                        currencyInstance.DOMove(currencyIcon.position,
                                addCurrencyTweenSetting.tweenDuration)
                            .SetEase(addCurrencyTweenSetting.moveCurve)
                            .OnComplete(() =>
                            {
                                _tweenCurrencyPool.Release(currencyInstance);
                                currencyInstance.gameObject.SetActive(false);
                                currencyInstance.transform.localPosition = Vector3.zero;
                                addCurrencyTweenSetting.DoPunch(currencyIcon.transform);
                            })).SetRecyclable(true);
            }

            //increment the coinsAmountText to the new amount
            var addingCurrencyDuration = sequence.Duration(false) - addCurrencyTweenSetting.tweenDuration;
            sequence.Insert(addCurrencyTweenSetting.tweenDuration,
                DOTween.To(() => CoinsAmount, x => CoinsAmount = x, PlayerData.coinsAmount,
                    addingCurrencyDuration).OnComplete(() =>
                {
                    _addingCurrency = false;
                    coinsAmountText.text = CoinsAmount.ToString();
                }));

            _addingCurrency = true;
        }
        
        private void DeductCurrency()
        {
            //instantiate a duplicate of the currency display (amount and icon) and tween it
            deductCurrencyPrefab.Init(_currencyDifference);
            //update the currency display to show the new amount
            CoinsAmount = PlayerData.coinsAmount;
            coinsAmountText.text = CoinsAmount.ToString();
        }
    }
}