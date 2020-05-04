using System.Collections.Generic;
using System.Linq;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.Managers
{
    public class UpgradeRepairManager : GlobalClass
    {
        private UpgradeRepairVariable[] _upgradeRepairVariables;

        private void Start()
        {
            LoadUpgradeRepairs();
        }

        private void LoadUpgradeRepairs()
        {
            var loadedList = Resources.LoadAll<UpgradeRepairVariable>(Constants.UpgradeRepairPath).ToList();
            loadedList.Sort((x, y) => x.id.CompareTo(y.id));
            _upgradeRepairVariables = loadedList.ToArray();
        }

        public int GetUpgradeRepairState(int id)
        {
            return PlayerData.upgradeRepairData[id];
        }
    }
}