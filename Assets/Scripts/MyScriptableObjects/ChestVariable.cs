using Enums;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Chest", menuName = "MyAssets/Chest", order = 65)]
    public class ChestVariable : ScriptableObject
    {
        public ChestType chestType;
        public Color chestColor;
        public string chestName;
        public Sprite chestIcon;
        public Sprite chestOpenIcon;
        public int rank;
        public int threshold;
        public int bcMax;
        public int bpMax;
        public int sfMax;
    }
}