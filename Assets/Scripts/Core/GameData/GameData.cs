using Firebase.Auth;
using UnityEngine;

namespace Core.GameData
{
    public class GameData : ScriptableObject
    {
        public FirebaseUser firebaseUser;

        private static GameData _instance;

        public static GameData Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindObjectOfType<GameData>();
                if (!_instance)
                    _instance = CreateDefaultGameData();
                return _instance;
            }
        }

        private static GameData CreateDefaultGameData()
        {
            var newGameData = (GameData) CreateInstance(typeof(GameData));
            newGameData.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return newGameData;
        }
    }
}