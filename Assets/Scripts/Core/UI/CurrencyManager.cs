using System;
using System.Collections;
using Core.Managers;
using Core.UI.Prefabs;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class CurrencyManager : GlobalClass
    {
        [Header("Shop")] [SerializeField] private Button bcShopButton;
        [SerializeField] private Button bpShopButton;
        [SerializeField] private Button sfShopButton;

        [Header("CurrencyUpdate")] public Currency[] currencies;
        public RectTransform currenciesRect;
        [SerializeField] private Transform hideTransform;
        [SerializeField] private AddCurrencyPrefab addCurrencyPrefab;

        [SerializeField] private TweenSetting addCurrencyTweenSetting;
        [SerializeField] private TweenSetting hideCurrencyTweenSetting;
        public Transform addCurrencySpawnPosition;
        public bool blockCurrencyRefresh;

        private MyObjectPool<AddCurrencyPrefab> _tweenCurrencyPool;
        private Vector2 _newSizeDelta = new Vector2();
        private bool _updateCurrencies;
        private bool _currenciesAreMoving;

        private void Start()
        {
            _tweenCurrencyPool =
                ObjectPoolManager.CreateObjectPool<AddCurrencyPrefab>(addCurrencyPrefab, addCurrencySpawnPosition);

            SetupCurrencies();

            bcShopButton.onClick.RemoveAllListeners();
            bcShopButton.onClick.AddListener(() => { OpenShopPanel(ResourceType.BananaCoins); });
            bpShopButton.onClick.RemoveAllListeners();
            bpShopButton.onClick.AddListener(() => { OpenShopPanel(ResourceType.BluePrints); });
            sfShopButton.onClick.RemoveAllListeners();
            sfShopButton.onClick.AddListener(() => { OpenShopPanel(ResourceType.StarFruits); });

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshCurrencyEvent,
                RefreshAllCurrencies);
        }

        private void Update()
        {
            if (!_updateCurrencies || blockCurrencyRefresh) return;

            UpdateCurrencies();

            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                if (!currencies[i].updateCurrency) continue;

                _updateCurrencies = true;
            }
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

        private static void OpenShopPanel(ResourceType resourceType)
        {
            if (PanelManager.OpenPanelCount() > 0)
                PanelManager.OpenSubPanel<ShopPanelController>(resourceType);
            else
                PanelManager.OpenPanelSolo<ShopPanelController>(resourceType);
        }

        private void RefreshAllCurrencies()
        {
            StartCoroutine(nameof(RefreshAllCurrenciesRoutine));
        }

        private IEnumerator RefreshAllCurrenciesRoutine()
        {
            if (_currenciesAreMoving)
                yield return new WaitUntil(() => !_currenciesAreMoving);

            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
            {
                RefreshCurrency(currencies[i]);
            }

            yield return null;
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

        private void AddCurrency(Currency currency)
        {
            var currencyInstanceAmount = GetSpawnAmount(currency.currencyDifference);
            ResizeCurrencyRect();

            var coinsSequence = DOTween.Sequence();
            for (var i = 0; i < currencyInstanceAmount; i++)
            {
                var currencyInstance = _tweenCurrencyPool.Get();
                currencyInstance.Init(currency.currencyIcon.sprite);
                currencyInstance.gameObject.SetActive(false);
                currencyInstance.transform.Rotate(Vector3.forward, Random.Range(-150.0f, -10.0f));
                currencyInstance.transform.localPosition = addCurrencyTweenSetting.RandomSpawnPosition();
                coinsSequence.InsertCallback(i * addCurrencyTweenSetting.delayBetweenInstance,
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
                            }))
                    .SetRecyclable(false);
            }

            var addingCurrencyDuration = coinsSequence.Duration(false) - addCurrencyTweenSetting.moveDuration;

            var currencyRef = currency; //isolation

            coinsSequence.Insert(addCurrencyTweenSetting.moveDuration,
                    DOTween.To(() => currencyRef.currencyDetails.resourceAmount,
                        x => currencyRef.currencyDetails.resourceAmount = x,
                        PlayerData.GetResourceAmount(currency.currencyDetails.resourceType), addingCurrencyDuration))
                .OnComplete(() =>
                {
                    currencyRef.UpdateTextDisplay();
                    currencyRef.updateCurrency = false;
                    EventManager.refreshUi.Raise();
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
            EventManager.refreshUi.Raise();
        }

        private IEnumerator HideCurrenciesRoutine(float moveOffset)
        {
            var t = 0.0f;
            var destination = hideTransform.localPosition;
            destination.y -= moveOffset;

            while (t < hideCurrencyTweenSetting.moveDuration)
            {
                hideTransform.localPosition = Vector2.Lerp(hideTransform.localPosition, destination,
                    t / hideCurrencyTweenSetting.moveDuration);
                t += Time.deltaTime;
                yield return null;
            }

            hideTransform.localPosition = destination;

            _currenciesAreMoving = false;

            yield return null;
        }
        
        private static int GetSpawnAmount(long difference)
        {
            if (difference > 2000)
                return 25;
            if (difference > 1000)
                return 20;
            if (difference > 200)
                return 15;
            if (difference > 9)
                return 10;

            return (int) difference;
        }

        public void HideCurrencies(bool value)
        {
            _currenciesAreMoving = true;
            
            var moveOffset = value ? hideCurrencyTweenSetting.moveOffset : -hideCurrencyTweenSetting.moveOffset;

            StartCoroutine(HideCurrenciesRoutine(moveOffset));
        }

        public long GetCurrencyAmount(ResourceType currencyType)
        {
            var currenciesLength = currencies.Length;
            for (var i = 0; i < currenciesLength; i++)
                if (currencies[i].currencyDetails.resourceType == currencyType)
                    return currencies[i].currencyDetails.resourceAmount;

            return 0;
        }
    }
}