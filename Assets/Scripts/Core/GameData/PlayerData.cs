using Firebase.Auth;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        public int coinsAmount;

        private void Start()
        {
            EventManager.NewEventSubscription(Constants.GameEvents.armPullEvent, DeductCoin);
            // var armPullEvent = Resources.Load<GameEvent>("Events/armPullEvent");
            // GameEventListener.NewGameEventListener(gameObject, armPullEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            coinsAmount -= 1;
        }
    }
}