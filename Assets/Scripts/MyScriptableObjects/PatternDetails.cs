using UnityEngine;

namespace MyScriptableObjects
{
    [System.Serializable]
    public class PatternDetails
    {
        public Vector3 startingOffset;
        public Vector2 widthHeight;
        public bool isGrid;
        public bool isCentered;
    }
}