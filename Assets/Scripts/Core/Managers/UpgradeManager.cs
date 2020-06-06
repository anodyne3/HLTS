using System.Collections;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.Managers
{
    public class UpgradeManager : GlobalClass
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TweenSetting showSliderTweenSetting;

        private UpgradeVariable[] _upgradeVariables;
        private bool _sliderIsActive;

        public override void Awake()
        {
            base.Awake();

            LoadUpgrades();
        }

        private void LoadUpgrades()
        {
            _upgradeVariables = GeneralUtils.SortLoadedList<UpgradeVariable>(Constants.UpgradesPath,
                (x, y) => x.upgradeType.CompareTo(y.upgradeType));

            RefreshUpgrades();
        }

        public void RefreshUpgrades()
        {
            if (PlayerData.upgradeData == null) return;

            var upgradeDataLength = PlayerData.upgradeData.Length;
            for (var i = 0; i < upgradeDataLength; i++)
                _upgradeVariables[i].currentLevel = PlayerData.upgradeData[i];

            EventManager.upgradeRefresh.Raise();
            EventManager.refreshCurrency.Raise();
            RefreshProgressSlider();
        }

        public int GetUpgradeCurrentLevel(UpgradeTypes id)
        {
            return _upgradeVariables[(int) id].currentLevel;
        }

        public UpgradeVariable GetUpgradeVariable(UpgradeTypes id)
        {
            return _upgradeVariables[(int) id];
        }

        public bool IsUpgradeMaxed(UpgradeTypes id)
        {
            return _upgradeVariables[(int) id].currentLevel >= _upgradeVariables[(int) id].maxLevel;
        }

        public bool HasResourcesForUpgrade(UpgradeTypes id)
        {
            var requiredResources = _upgradeVariables[(int) id].CurrentResourceRequirements;

            if (requiredResources == null)
                return false;

            var enoughResources = true;

            var requiredResourcesLength = requiredResources.Length;
            for (var i = 0; i < requiredResourcesLength; i++)
            {
                if (CurrencyManager.GetCurrencyAmount(requiredResources[i].resourceType) >=
                    requiredResources[i].resourceAmount)
                    continue;

                enoughResources = false;
            }

            return enoughResources;
        }

        private void RefreshProgressSlider()
        {
            ShowProgressSlider();

            progressSlider.DOValue(CurrentUpgradeProgress(), showSliderTweenSetting.scaleDuration)
                .SetEase(showSliderTweenSetting.sequenceEasing);
        }

        private float CurrentUpgradeProgress()
        {
            int currentLevels = 0, maxCount = 0;

            var upgradeVariablesLength = _upgradeVariables.Length;
            for (var i = 0; i < upgradeVariablesLength; i++)
            {
                currentLevels += _upgradeVariables[i].currentLevel;
                maxCount += _upgradeVariables[i].maxLevel;
            }

            return currentLevels / (float) maxCount;
        }

        private void ShowProgressSlider()
        {
            if (CurrentUpgradeProgress() < Constants.FloatTolerance) return;

            if (_sliderIsActive) return;

            StartCoroutine(nameof(HideCurrenciesRoutine));
        }

        private IEnumerator HideCurrenciesRoutine()
        {
            var t = 0.0f;
            var destination = progressSlider.transform.localPosition;
            destination.x += showSliderTweenSetting.moveOffset;

            while (t < showSliderTweenSetting.moveDuration)
            {
                progressSlider.transform.localPosition = Vector2.Lerp(progressSlider.transform.localPosition,
                    destination,
                    t / showSliderTweenSetting.moveDuration);
                t += Time.deltaTime;
                yield return null;
            }

            progressSlider.transform.localPosition = destination;
            _sliderIsActive = true;

            yield return null;
        }

        public bool IsSliderActive()
        {
            return _sliderIsActive;
        }
    }
}