using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Chest", menuName = "MyAssets/Chest", order = 65)]
    public class ChestVariable : ScriptableObject
    {
        public ChestType chestType;
        public Color chestColor;
        public Sprite chestIcon;
    }
}