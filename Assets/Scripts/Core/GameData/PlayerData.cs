using Firebase.Auth;
using MyScriptableObjects;
using UnityEngine;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        public int coinsAmount;

        private void Start()
        {
            var armPullEvent = Resources.Load<GameEvent>("Events/armPullEvent");
            GameEventListener.NewGameEventListener(gameObject, armPullEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            coinsAmount -= 1;
        }
    }
}