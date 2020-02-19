using Firebase.Auth;
using UnityEngine;

namespace Core.GameData
{
    public class GlobalData : ScriptableObject
    {
        public FirebaseUser firebaseUser;
        
        private static GlobalData _instance;

        public static GlobalData Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<GlobalData>();
                if (!_instance)
                    _instance = CreateDefaultGlobalData();
                return _instance;
            }
        }

        private static GlobalData CreateDefaultGlobalData()
        {
            var newGlobalData = (GlobalData) CreateInstance(typeof(GlobalData));
            newGlobalData.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return newGlobalData;
        }
    }
}