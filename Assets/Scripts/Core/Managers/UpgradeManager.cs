using System.Linq;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.Managers
{
    public class UpgradeManager : GlobalClass
    {
        private UpgradeVariable[] _upgradeVariables;

        public long upgradeId;

        public override void Awake()
        {
            base.Awake();

            LoadUpgrades();
        }

        private void LoadUpgrades()
        {
            var loadedList = Resources.LoadAll<UpgradeVariable>(Constants.UpgradesPath).ToList();
            loadedList.Sort((x, y) => x.id.CompareTo(y.id));
            _upgradeVariables = loadedList.ToArray();
            RefreshUpgrades();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent, RefreshUpgrades);
        }

        private void RefreshUpgrades()
        {
            if (PlayerData.upgradeData == null) return;

            var upgradeDataLength = PlayerData.upgradeData.Length;
            for (var i = 0; i < upgradeDataLength; i++)
                _upgradeVariables[i].currentLevel = PlayerData.upgradeData[i];

            EventManager.refreshUi.Raise();
        }

        public int GetUpgradeCurrentLevel(int id)
        {
            return _upgradeVariables[id].currentLevel;
        }

        public UpgradeVariable GetUpgradeVariable(int id)
        {
            return _upgradeVariables[id];
        }

        public bool IsUpgradeMaxed(int id)
        {
            return _upgradeVariables[id].currentLevel >= _upgradeVariables[id].maxLevel;
        }

        public bool HasResourcesForUpgrade(int id)
        {
            var requiredResources = _upgradeVariables[id].resourceRequirements; 
            
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