using Core.UI;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "UpgradeRepairVariable", menuName = "UpgradeRepairVariable", order = 70)]
    public class UpgradeRepairVariable : ScriptableObject
    {
        public bool upgrade;
        public Sprite icon;
        public string deviceName;
        public string description;
        public Resource[] resourceRequirements;
    }
}