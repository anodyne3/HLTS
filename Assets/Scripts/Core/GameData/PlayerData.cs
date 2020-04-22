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
        public static bool ConsentGiven => PlayerPrefs.HasKey(Constants.ConsentKey) && PlayerPrefs.GetInt(Constants.ConsentKey) == 1;
        public FirebaseUser firebaseUser;
        [HideInInspector] public long coinsAmount = 10;
        [HideInInspector] public int[] lastResult;
        [HideInInspector] public int[] nextResult;
        public int currentChestRoll;

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
        
        public void StopDatabaseListeners()
        {
            if (firebaseUser == null || firebaseUser.UserId == string.Empty) return;
            
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).ValueChanged -= OnPlayerDataChanged;
        }

        private void OnDisable()
        {
            StopDatabaseListeners();
        }

        private void OnPlayerDataChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender != null)
                Debug.Log(sender.ToString());
            
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (string.IsNullOrEmpty(args.Snapshot.GetRawJsonValue()))
            {
                Debug.Log("Empty Snapshot: " + args.Snapshot.Key);
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