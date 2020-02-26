using Firebase.Auth;
using Utils;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        //temp
        public int coinsAmount = 50;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            //make this a database change 
            coinsAmount -= 1;
            EventManager.refreshUi.Raise();
        }
        
        public void AddPayout(int value)
        {
            //make this a database change 
            coinsAmount += value;
            EventManager.refreshUi.Raise();
        }

        public int GetPlayerCoinsAmount()
        {
            return coinsAmount;
        }
    }
}