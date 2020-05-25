using System;
using Core.UI;
using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Upgrade", menuName = "MyAssets/Variables/Upgrade", order = 20)]
    public class UpgradeVariable : ScriptableObject
    {
        public UpgradeTypes upgradeType;
        public int currentLevel;
        public int maxLevel;
        [SerializeField] private UpgradeLevel[] levelDetails;
        public Sprite CurrentIcon => levelDetails[currentLevel].icon;
        //need to fix this, index out of array
        public string CurrentUpgradeName => levelDetails[currentLevel].upgradeName;
        public string CurrentDescription => levelDetails[currentLevel].description;
        public Resource[] CurrentResourceRequirements => currentLevel < levelDetails.Length
            ? levelDetails[currentLevel].resourceRequirements
            : null;
    }
}

[Serializable]
public class UpgradeLevel
{
    public Sprite icon;
    public string upgradeName;
    public string description;
    public Resource[] resourceRequirements;
}