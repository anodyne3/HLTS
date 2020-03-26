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
    public class HudController : GlobalAccess
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private TMP_Text coinsAmountText;

        [Header("CurrencyUpdate")]
        [SerializeField] private Transform currencyPrefab;
        [SerializeField] private Transform currencyIcon;
        [SerializeField] private Transform currencySpawnPosition;
        [SerializeField] private TweenSetting addCurrencyTweenSetting;

        private MyObjectPool<Transform> _tweenCurrencyPool;
        private long _coinsAmount;
        private long _currencyDifference;
        private bool _addingCurrency;

        private void Start()
        {
            _tweenCurrencyPool = ObjectPoolManager.CreateObjectPool<Transform>(currencyPrefab, currencySpawnPosition);

            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);
            RefreshCoins();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshCoins);
        }

        private void Update()
        {
            if (!_addingCurrency) return;
            
            coinsAmountText.text = _coinsAmount.ToString();
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
            ResizeCoinsAmountTextBackground();
            _currencyDifference = PlayerData.coinsAmount - _coinsAmount;

            if (_currencyDifference == 0) return;

            if (_currencyDifference > 0)
                AddCurrency();
            else
                DeductCurrency();
        }

        //resize the width of the coin currency background according to the number of digits 
        private void ResizeCoinsAmountTextBackground()
        {
            var rect = (RectTransform) coinsAmountText.transform.parent.parent.GetComponent(typeof(RectTransform));
            var digitCount = Math.Floor(Math.Log10(PlayerData.coinsAmount) + 1);
            var newSizeDelta = new Vector2(double.IsInfinity(digitCount)
                ? Constants.CoinsBackgroundBaseWidth + Constants.CoinsBackgroundWidthMultiplier
                : (float) digitCount * Constants.CoinsBackgroundWidthMultiplier +
                  Constants.CoinsBackgroundBaseWidth, rect.sizeDelta.y);
            
            rect.DOSizeDelta(newSizeDelta, Constants.SizeDeltaTweenDuration);
        }

        private void AddCurrency()
        {
            //get a bunch of coins from the pool and move them to the currency target
            var currencyInstanceAmount = TweenSetting.GetSpawnAmount(_currencyDifference);
            
            var sequence = DOTween.Sequence();
            for (var i = 0; i < currencyInstanceAmount; i++)
            {
                var currencyInstance = _tweenCurrencyPool.Get();
                currencyInstance.gameObject.SetActive(false);
                currencyInstance.transform.Rotate(Vector3.forward, Random.Range(-150.0f, -10.0f));
                currencyInstance.transform.localPosition = addCurrencyTweenSetting.RandomSpawnPosition();
                sequence.InsertCallback(i * addCurrencyTweenSetting.delayBetweenInstance, () => currencyInstance.gameObject.SetActive(true))
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
                            }));
            }

            //increment the coinsAmountText to the new amount
            var addingCurrencyDuration = sequence.Duration(false) - addCurrencyTweenSetting.tweenDuration;
            sequence.Insert(addCurrencyTweenSetting.tweenDuration,
                DOTween.To(() => _coinsAmount, x => _coinsAmount = x, PlayerData.coinsAmount,
                    addingCurrencyDuration).OnComplete(() =>
                {
                    _addingCurrency = false;
                    coinsAmountText.text = _coinsAmount.ToString();
                }));

            _addingCurrency = true;
        }

        private void DeductCurrency()
        {
            //instantiate a duplicate of the currency display (amount and icon)
            //move the duplicate down and fade it to zero
            //update the currency display to show the new amount
            coinsAmountText.text = _coinsAmount.ToString();
        }
    }
}