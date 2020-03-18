using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "floatVariable", menuName = "MyAssets/FloatVariable", order = 40)]
    public class FloatVariable : ScriptableObject
    {
        public float value;
    }
}