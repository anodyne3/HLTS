using System.Collections;
using Core.Managers;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Utils;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        public long coinsAmount;
        public int[] lastResult;
        public int[] nextResult;

        private FirebaseDatabase _database;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            coinsAmount -= 1;
            EventManager.refreshUi.Raise();
        }

        private void StartDatabaseListeners()
        {
            _database = FirebaseDatabase.DefaultInstance;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).ValueChanged += OnPlayerDataChanged;
        }

        private void OnDisable()
        {
            if (firebaseUser == null) return;
            
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).ValueChanged -= OnPlayerDataChanged;
        }

        private void OnPlayerDataChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            var snapReturn = args.Snapshot.GetRawJsonValue();
            var snapReturnDict = new PlayerDataDto(snapReturn);
            lastResult = snapReturnDict.lastResult.ToArray();
            nextResult = snapReturnDict.nextResult.ToArray();
            coinsAmount = snapReturnDict.coinsAmount;
        }

        public IEnumerator OnLogin()
        {
            StartDatabaseListeners();

            yield return new WaitUntil(() => lastResult != null);
            
            GameManager.LoadMain();
        }
    }
}