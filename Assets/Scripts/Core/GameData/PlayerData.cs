using Firebase.Auth;
using Utils;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        public int coinsAmount;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            //make this a database change 
            coinsAmount -= 1;
        }
    }
}