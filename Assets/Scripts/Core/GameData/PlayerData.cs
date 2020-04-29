using System.Collections;
using Core.Managers;
using Core.UI;
using Enums;
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
        [HideInInspector] public long bcAmount = 10;
        [HideInInspector] public long bpAmount;
        [HideInInspector] public long sfAmount;
        [HideInInspector] public int[] lastResult;
        [HideInInspector] public int[] nextResult;
        [HideInInspector] public int[] chestData;
        
        public int currentChestRoll;
        
        private FirebaseDatabase _database;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            bcAmount -= 1;
            EventManager.refreshUi.Raise();
        }

        private void StartDatabaseListeners()
        {
            _database = FirebaseDatabase.DefaultInstance;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).ValueChanged += OnPlayerDataChanged;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.ChestDataSuffix).ValueChanged += OnChestDataChanged;
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
            var snapReturnDto = new PlayerDataDto(snapReturn);
            lastResult = snapReturnDto.lastResult.ToArray();
            nextResult = snapReturnDto.nextResult.ToArray();
            bcAmount = snapReturnDto.coinsAmount;
        }

        private void OnChestDataChanged(object sender, ValueChangedEventArgs args)
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
            var snapReturnDto = new ChestDto(snapReturn);
            chestData = snapReturnDto.newChestData;
        }

        public IEnumerator OnLogin()
        {
            StartDatabaseListeners();

            yield return new WaitUntil(() => lastResult != null);
            
            GameManager.LoadMain();
        }

        #region ChestData

        public int GetChestCount(ChestType chestType)
        {
            if (chestData == null)
                chestData = new[] {0, 0, 0};
            
            return chestData[(int) chestType];
        }

        #endregion
    }
}