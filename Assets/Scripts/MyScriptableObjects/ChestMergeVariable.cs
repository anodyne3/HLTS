using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "ChestMergeVariable", menuName = "MyAssets/Variables/ChestMerge", order = 30)]
    public class ChestMergeVariable : ScriptableObject
    {
        public int mergeUpgradeLevel; 
        public ChestType requiredType;
        public int requiredAmount;
        public ChestType receivedType;
        public int receivedAmount;
    }
}