using System.Collections.Generic;
using System.Linq;
using Core.GameData;
using Core.UI;
using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;
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

        public ChestVariable[] chestTypes;

        public ChestVariable CurrentChest
        {
            get
            {
                var chestTypesLength = chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i++)
                {
                    if (chestTypes[i].chestType == CurrentChestType)
                        return chestTypes[i];
                }

                return null;
            }
        }

        public int RollsToBetterChest => CurrentChest.threshold - PlayerData.currentChestRoll;

        private ChestType CurrentChestType
        {
            get
            {
                var chestTypesLength = chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i ++)
                {
                    if (PlayerData.currentChestRoll > chestTypes[i].threshold) continue;

                    return chestTypes[i].chestType;
                }

                return chestTypes[0].chestType;
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
            var loadedChests = Resources.LoadAll<ChestVariable>(Constants.ChestsPath).ToList();
            loadedChests.Sort((x, y) => x.rank.CompareTo(y.rank));
            chestTypes = loadedChests.ToArray();
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

        public void OpenChest(ChestRewardDto chestRewardDto)
        {
            PanelManager.OpenSubPanel<OpenChestPanelController>(chestRewardDto);
        }  

        public float GetFillAmount()
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

        public Sprite GetChestIcon(ChestType chestType)
        {
            var chestTypesLength = chestTypes.Length;
            for (var i = 0; i < chestTypesLength; i++)
            {
                if (chestTypes[i].chestType == chestType)
                    return chestTypes[i].chestIcon;
            }

            return null;
        }
    }
}