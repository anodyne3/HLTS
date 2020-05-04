using Core.UI;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "UpgradeRepairVariable", menuName = "UpgradeRepairVariable", order = 70)]
    public class UpgradeRepairVariable : ScriptableObject
    {
        public int id;
        public int currentLevel;
        public int maxLevel;
        public bool IsUpgrade => maxLevel > 1;
        public Sprite icon;
        public string deviceName;
        public string description;
        public Resource[] resourceRequirements;
    }
}