using System;
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
        public static bool ConsentGiven =>
            PlayerPrefs.HasKey(Constants.ConsentKey) && PlayerPrefs.GetInt(Constants.ConsentKey) == 1;

        [HideInInspector] public int[] lastResult;
        [HideInInspector] public int[] nextResult;
        [HideInInspector] public int[] chestData = {1,2,3};
        [HideInInspector] public int[] upgradeData = {1,0};

        public FirebaseUser firebaseUser;
        public Resource[] wallet =
        {
            new Resource(_bcAmount, ResourceType.BananaCoins),
            new Resource(_bpAmount, ResourceType.BluePrints),
            new Resource(_sfAmount, ResourceType.StarFruits)
        };

        public int currentChestRoll;

        private FirebaseDatabase _database;
        //for testing
        private static long _bcAmount = 100;
        private static long _bpAmount = 10;
        private static long _sfAmount = 1;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            wallet[0].resourceAmount -= 1;
            EventManager.refreshUi.Raise();
        }

        private void StartDatabaseListeners()
        {
            _database = FirebaseDatabase.DefaultInstance;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).ValueChanged += OnPlayerDataChanged;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).Child(Constants.ChestData).ValueChanged += OnChestDataChanged;
            _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix).Child(Constants.UpgradeData).ValueChanged += OnUpgradeDataChanged;
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
            /*if (sender != null)
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

            var snapReturn = args.Snapshot.GetRawJsonValue();*/
            var snapReturnDto = new PlayerDataDto(ProcessDataChanges(sender, args));
            lastResult = snapReturnDto.lastResult.ToArray();
            nextResult = snapReturnDto.nextResult.ToArray();
            wallet[0].resourceAmount = snapReturnDto.coinsAmount;
        }

        private void OnChestDataChanged(object sender, ValueChangedEventArgs args)
        {
            chestData = new GenericArrayDto(ProcessDataChanges(sender, args)).newDataArray;
            // ProcessGenericArrayDataChanges(sender, args, ref chestData);
        }
        
        private void OnUpgradeDataChanged(object sender, ValueChangedEventArgs args)
        {
            // ProcessGenericArrayDataChanges(sender, args, ref upgradeData);
            upgradeData = new GenericArrayDto(ProcessDataChanges(sender, args)).newDataArray;
        }

        /*private static void ProcessGenericArrayDataChanges(object sender, ValueChangedEventArgs args, ref int[] dataChanged)
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
            var snapReturnDto = new GenericArrayDto(snapReturn);
            dataChanged = snapReturnDto.newDataArray;
        }*/
        
        private static string ProcessDataChanges(object sender, ValueChangedEventArgs args)
        {
            if (sender != null)
                Debug.Log(sender.ToString());

            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(args.Snapshot.GetRawJsonValue()))
                return args.Snapshot.GetRawJsonValue();
            
            Debug.Log("Empty Snapshot: " + args.Snapshot.Key);
            return string.Empty;
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
            return chestData[(int) chestType];
        }

        #endregion

        #region Resources

        public long GetResourceAmount(ResourceType currencyType)
        {
            var walletLength = wallet.Length;
            for (var i = 0; i < walletLength; i++)
            {
                var currency = wallet[i];
                if (currency.resourceType == currencyType)
                {
                    return currency.resourceAmount;
                }
            }

            return 0;
        }

        public void AddResourceAmount(Resource currency)
        {
            var walletLength = wallet.Length;
            for (var i = 0; i < walletLength; i++)
            {
                if (wallet[i].resourceType != currency.resourceType) continue;
                
                wallet[i].resourceAmount += currency.resourceAmount;
            }
        }

        #endregion
    }
}