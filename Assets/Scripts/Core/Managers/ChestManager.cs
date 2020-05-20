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
        [SerializeField] private ChestAddedPrefab chestAddedPrefab;
        [SerializeField] private Transform chestAddedHolder;

        private MyObjectPool<ChestAddedPrefab> _tweenChestAddedPool;
        public ChestVariable[] chestTypes;
        public ChestMergeVariable[] chestMergeTypes;

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

        public int RollsToBetterChest
        {
            get
            {
                if (PlayerData.currentChestRoll == CurrentChest.threshold)
                    return GetChestVariable(CurrentChest.rank + 1).threshold - PlayerData.currentChestRoll;

                return CurrentChest.threshold - PlayerData.currentChestRoll;
            }
        }

        private ChestType CurrentChestType
        {
            get
            {
                var chestTypesLength = chestTypes.Length;
                for (var i = 0; i < chestTypesLength; i++)
                {
                    if (PlayerData.currentChestRoll > chestTypes[i].threshold)
                        continue;

                    return chestTypes[i].chestType;
                }

                return chestTypes[0].chestType;
            }
        }

        private void Start()
        {
            var openChestPanel = PanelManager.GetPanel<ChestOpenPanelController>();
            openChestPanel.chestRewardPool =
                ObjectPoolManager.CreateObjectPool<ChestRewardPrefab>(openChestPanel.chestRewardPrefab,
                    openChestPanel.rewardStartPosition);
            _tweenChestAddedPool =
                ObjectPoolManager.CreateObjectPool<ChestAddedPrefab>(chestAddedPrefab, chestAddedHolder);

            LoadChests();
            RefreshChest();
            RefreshFill();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshFill);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, RefreshChest);
        }

        private void LoadChests()
        {
            chestTypes =
                GeneralUtils.SortLoadedList<ChestVariable>(Constants.ChestsPath,
                    (x, y) => x.rank.CompareTo(y.rank));
            chestMergeTypes =
                GeneralUtils.SortLoadedList<ChestMergeVariable>(Constants.ChestMergesPath,
                    (x, y) => x.mergeUpgradeLevel.CompareTo(y.mergeUpgradeLevel));
        }

        private void RefreshChest()
        {
            chestIcon.sprite = CurrentChest.chestIcon;
            outlineImage.color = CurrentChest.chestColor;
        }

        private void RefreshFill()
        {
            chestProgressFillImage.DOFillAmount(GetFillAmount(CurrentChest.rank), tweenPunchSetting.punchDuration);

            var tweenPause = DOTween.Sequence();

            if (PlayerData.currentChestRoll != CurrentChest.threshold) return;

            tweenPause.InsertCallback(0.5f, () =>
            {
                UpgradeChest();
                chestProgressFillImage.DOFillAmount(GetFillAmount(CurrentChest.rank + 1),
                    tweenPunchSetting.punchDuration);
            });
        }

        private void UpgradeChest()
        {
            chestIcon.sprite = GetChestVariable(CurrentChest.rank + 1).chestIcon;
            outlineImage.color = GetChestVariable(CurrentChest.rank + 1).chestColor;
            tweenPunchSetting.DoPunch(transform);
        }

        public static void OpenChest(ChestRewardDto chestRewardDto)
        {
            if (chestRewardDto == null) return;

            PanelManager.OpenSubPanel<ChestOpenPanelController>(chestRewardDto);
        }

        public float GetFillAmount(int rank)
        {
            return PlayerData.currentChestRoll / (float) GetChestVariable(rank).threshold;
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

        public ChestVariable GetChestVariable(int rank)
        {
            if (rank < 0)
                return chestTypes[0];

            return rank < chestTypes.Length ? chestTypes[rank] : CurrentChest;
        }

        public void ChestClaimed(ChestType chestType)
        {
            AlertMessage.Init(chestType + Constants.ClaimMessage);
            EventManager.chestRefresh.Raise();
        }

        public static void CompleteMerge()
        {
            AlertMessage.Init(Constants.MergeMessage);
        }

        #region ChestAddedAnim

        public void AddChestsAnim(ChestType chestType, int amount)
        {
            var sequence = DOTween.Sequence();
            
            for (var i = 0; i < amount; i++)
            {
                sequence.AppendCallback(() =>
                {
                    ChestAddedAnim(chestType);
                })
                    .AppendInterval(Constants.ChestAddInterval);
            }
        }

        private void ChestAddedAnim(ChestType chestType)
        {
            var chestAddedInstance = _tweenChestAddedPool.Get();
            chestAddedInstance.transform.SetAsLastSibling();
            chestAddedInstance.Init(chestType);
            chestAddedInstance.gameObject.SetActive(true);
        }

        public void ChestAddedAnimComplete(ChestAddedPrefab completedPrefab)
        {
            _tweenChestAddedPool.Release(completedPrefab);
            completedPrefab.gameObject.SetActive(false);
            tweenPunchSetting.DoPunch(outlineImage.transform);
        }
        
        #endregion
    }
}