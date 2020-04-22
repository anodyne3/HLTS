using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.Managers
{
    public class ChestManager : GlobalClass
    {
        [SerializeField] private Image chestIcon;
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image chestProgressFillImage;
        [SerializeField] private TweenPunchSetting tweenPunchSetting;

        private ChestVariable[] _chestTypes;

        private ChestVariable CurrentChest
        {
            get
            {
                var chestTypesLength = _chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i++)
                {
                    if (_chestTypes[i].chestType == CurrentChestType)
                        return _chestTypes[i];
                }

                return null;
            }
        }

        public static int RollsToBetterChest
        {
            get
            {
                if (PlayerData.currentChestRoll < Constants.LoChestRoll)
                    return Constants.LoChestRoll - PlayerData.currentChestRoll;
                if (PlayerData.currentChestRoll == Constants.LoChestRoll)
                    return 0;
                if (PlayerData.currentChestRoll < Constants.MiChestRoll)
                    return Constants.MiChestRoll - PlayerData.currentChestRoll;
                if (PlayerData.currentChestRoll == Constants.MiChestRoll)
                    return 0;
                if (PlayerData.currentChestRoll < Constants.HiChestRoll)
                    return Constants.HiChestRoll - PlayerData.currentChestRoll;

                return 0;
            }
        }

        private static ChestType CurrentChestType
        {
            get
            {
                if (PlayerData.currentChestRoll < Constants.LoChestRoll)
                    return ChestType.Lo;
                
                return PlayerData.currentChestRoll < Constants.MiChestRoll ? ChestType.Mi : ChestType.Hi;
            }
        }

        private void Start()
        {
            LoadChests();
            RefreshChest();
            RefreshFill();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshFill);
        }

        private void LoadChests()
        {
            _chestTypes = Resources.LoadAll<ChestVariable>(Constants.ChestsPath);
        }

        private void RefreshChest()
        {
            chestIcon.sprite = CurrentChest.chestIcon;
            outlineImage.color = CurrentChest.chestColor;
        }

        private void RefreshFill()
        {
            chestProgressFillImage.DOFillAmount(GetFillAmount(), tweenPunchSetting.punchDuration);

            var tweenPause = DOTween.Sequence();
            tweenPause.InsertCallback(0.5f, () =>
            {
                if (RollsToBetterChest == 0)
                    UpgradeChest();
            });
        }

        private void UpgradeChest()
        {
            RefreshChest();
            chestProgressFillImage.fillAmount = 0.0f;
            tweenPunchSetting.DoPunch(transform);
        }

        private static float GetFillAmount()
        {
            switch (CurrentChestType)
            {
                default:
                    return (Constants.LoChestRoll - (float) RollsToBetterChest) / Constants.LoChestRoll;
                case ChestType.Mi:
                    return (Constants.MiChestRoll - (float) RollsToBetterChest) / Constants.MiChestRoll;
                case ChestType.Hi:
                    return (Constants.HiChestRoll - (float) RollsToBetterChest) / Constants.HiChestRoll;
            }
        }
    }
}