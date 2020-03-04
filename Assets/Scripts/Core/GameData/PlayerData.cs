using Core.Managers;
using Firebase.Auth;
using Firebase.Database;
using Utils;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public static FirebaseUser FirebaseUser => FirebaseFunctionality.firebaseUser;
        public long coinsAmount;

        private FirebaseDatabase _database;

        private void Start()
        {
            _database = FirebaseDatabase.DefaultInstance;
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        public async void OnLogin()
        {
            var rootDataTask = _database
                .GetReferenceFromUrl("https://he-loves-the-slots.firebaseio.com/users/" + FirebaseUser.UserId +
                                     "/userData/coinsAmount").GetValueAsync();
            await rootDataTask;

            if (rootDataTask.IsCompleted)
            {
                var snapshotValue = rootDataTask.Result.GetValue(false);
                coinsAmount = (long) snapshotValue;
            }

            GameManager.LoadMain();
        }

        private void DeductCoin()
        {
            coinsAmount -= 1;
            EventManager.refreshUi.Raise();
        }

        public void AddPayout(int value)
        {
            coinsAmount += value;
            EventManager.refreshUi.Raise();
        }

        public long GetPlayerCoinsAmount()
        {
            return coinsAmount;
        }
    }
}