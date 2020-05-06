using System.Linq;
using Core.GameData;
using Core.UI;
using Core.UI.Prefabs;
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
        
        public ChestVariable[] chestTypes;

        public ChestVariable CurrentChest
        {
            get
            {
                if (chestTypes == null) return null;
                var chestTypesLength = chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i++)
                    if (chestTypes[i].chestType == CurrentChestType)
                        return chestTypes[i];

                return null;
            }
        }

        public int RollsToBetterChest => CurrentChest.threshold - PlayerData.currentChestRoll;

        private ChestType CurrentChestType
        {
            get
            {
                var chestTypesLength = chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i++)
                {
                    if (PlayerData.currentChestRoll <= chestTypes[i].threshold)
                        return chestTypes[i].chestType;
                }

                return chestTypes[0].chestType;
            }
        }

        //kill this - will come from firebase
        [SerializeField] public ChestRewardDto chestReward;
        [ContextMenu("OpenChestTest")]
        public void OpenChestTest()
        {
            chestReward = new ChestRewardDto("[60,12,3]");
            OpenChest(chestReward);
        }

        private void Start()
        {
            var openChestPanel = PanelManager.GetPanel<OpenChestPanelController>();
            openChestPanel.chestRewardPool =
                ObjectPoolManager.CreateObjectPool<ChestRewardPrefab>(openChestPanel.chestRewardPrefab,
                    openChestPanel.rewardStartPosition);
            
            LoadChests();
            RefreshChest();
            RefreshFill();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshFill);
            //create refreshUpgradesEvent to reload and refresh chests after an upgrade
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
                if (PlayerData.currentChestRoll == CurrentChest.threshold)
                    UpgradeChest();
            });
        }

        private void UpgradeChest()
        {
            chestIcon.sprite = chestTypes[CurrentChest.rank + 1].chestIcon;
            outlineImage.color = chestTypes[CurrentChest.rank + 1].chestColor;
            tweenPunchSetting.DoPunch(transform);
            GetFillAmount();
        }

        public void OpenChest(ChestRewardDto chestRewardDto)
        {
            PanelManager.OpenSubPanel<OpenChestPanelController>(chestRewardDto);
        }

        public float GetFillAmount()
        {
            return (CurrentChest.threshold - (float) RollsToBetterChest) / CurrentChest.threshold;
            
            /*switch (CurrentChestType)
            {
                default:
                    return (Constants.LoChestRoll - (float) RollsToBetterChest) / Constants.LoChestRoll;
                case ChestType.Mi:
                    return (Constants.MiChestRoll - (float) RollsToBetterChest) / Constants.MiChestRoll;
                case ChestType.Hi:
                    return (Constants.HiChestRoll - (float) RollsToBetterChest) / Constants.HiChestRoll;
            }*/
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