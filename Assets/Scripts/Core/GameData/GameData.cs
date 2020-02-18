using UnityEngine;

namespace Core.GameData
{
    public class GameData : ScriptableObject
    {
        public int playerId;
        public int playerLevel;

        private static GameData _instance;

        public GameData Instance
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
            return newGameData;
        }
    }
}