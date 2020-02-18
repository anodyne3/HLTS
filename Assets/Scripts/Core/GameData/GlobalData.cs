using UnityEngine;

namespace Core.GameData
{
    public class GlobalData : ScriptableObject
    {
        public GameData GameData => Foundation.GameData;

    }
}