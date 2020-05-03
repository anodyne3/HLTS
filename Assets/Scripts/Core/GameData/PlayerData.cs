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

        public FirebaseUser firebaseUser;
        private static long _bcAmount = 100;
        private static long _bpAmount = 10;
        private static long _sfAmount = 1;
        [HideInInspector] public int[] lastResult;
        [HideInInspector] public int[] nextResult;
        [HideInInspector] public int[] chestData = {1,2,3};

        public Currency[] wallet =
        {
            new Currency(_bcAmount, CurrencyType.BananaCoins),
            new Currency(_bpAmount, CurrencyType.BluePrints),
            new Currency(_sfAmount, CurrencyType.StarFruits)
        };

        public int currentChestRoll;

        private FirebaseDatabase _database;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinConsumeEvent, DeductCoin);
        }

        private void DeductCoin()
        {
            wallet[0].currencyAmount -= 1;
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
            _bcAmount = snapReturnDto.coinsAmount;
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
            return chestData[(int) chestType];
        }

        #endregion

        #region Resources

        public long GetResourceAmount(CurrencyType currencyType)
        {
            var walletLength = wallet.Length;
            for (var i = 0; i < walletLength; i++)
            {
                var currency = wallet[i];
                if (currency.currencyType == currencyType)
                {
                    return currency.currencyAmount;
                }
            }

            return 0;
        }
        
        public void SetResourceAmount(CurrencyType currencyType, long amount)
        {
            var walletLength = wallet.Length;
            for (var i = 0; i < walletLength; i++)
            {
                var currency = wallet[i];
                if (currency.currencyType == currencyType)
                {
                    currency.currencyAmount += amount;
                }
            }
        }
        
        public void SetResourceAmount(Currency currency)
        {
            var walletLength = wallet.Length;
            for (var i = 0; i < walletLength; i++)
            {
                if (wallet[i].currencyType != currency.currencyType) continue;
                
                wallet[i].currencyAmount += currency.currencyAmount;
            }
        }

        #endregion
    }
}