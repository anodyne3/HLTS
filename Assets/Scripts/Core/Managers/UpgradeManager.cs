using Enums;
using MyScriptableObjects;
using Utils;

namespace Core.Managers
{
    public class UpgradeManager : GlobalClass
    {
        private UpgradeVariable[] _upgradeVariables;

        public override void Awake()
        {
            base.Awake();

            LoadUpgrades();
        }

        private void LoadUpgrades()
        {
            _upgradeVariables = GeneralUtils.SortLoadedList<UpgradeVariable>(Constants.UpgradesPath,
                (x, y) => x.upgradeType.CompareTo(y.upgradeType));

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent, RefreshUpgrades,
                true);
        }

        private void RefreshUpgrades()
        {
            if (PlayerData.upgradeData == null) return;

            var upgradeDataLength = PlayerData.upgradeData.Length;
            for (var i = 0; i < upgradeDataLength; i++)
                _upgradeVariables[i].currentLevel = PlayerData.upgradeData[i];

            EventManager.refreshUi.Raise();
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

            var enoughResources = true;

            var requiredResourcesLength = requiredResources.Length;
            for (var i = 0; i < requiredResourcesLength; i++)
            {
                if (PlayerData.GetResourceAmount(requiredResources[i].resourceType) >=
                    requiredResources[i].resourceAmount)
                    continue;

                enoughResources = false;
            }

            return enoughResources;
        }
    }
}