using System.Linq;
using MyScriptableObjects;
using UnityEngine;
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
            var loadedList = Resources.LoadAll<UpgradeVariable>(Constants.UpgradesPath).ToList();
            loadedList.Sort((x, y) => x.id.CompareTo(y.id));
            _upgradeVariables = loadedList.ToArray();
            RefreshUpgrades();
        }

        public void RefreshUpgrades()
        {
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
    }
}