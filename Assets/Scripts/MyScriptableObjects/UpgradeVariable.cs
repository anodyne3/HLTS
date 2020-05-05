using Core.UI;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Upgrade", menuName = "MyAssets/Variables/Upgrade", order = 20)]
    public class UpgradeVariable : ScriptableObject
    {
        public int id;
        public int currentLevel;
        public int maxLevel;
        public bool IsUpgrade => maxLevel > 1 && currentLevel > 0;
        public Sprite icon;
        public string upgradeName;
        public string description;
        public Resource[] resourceRequirements;
    }
}