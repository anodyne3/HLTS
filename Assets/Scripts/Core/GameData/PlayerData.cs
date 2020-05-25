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
        [HideInInspector] public int[] chestData = {1,2,3};
        [HideInInspector] public int[] chestPayout;
        [HideInInspector] public int currentChestRoll;
        [HideInInspector] public int[] lastResult;
        [HideInInspector] public int[] nextResult;
        [HideInInspector] public int[] upgradeData;

        public Resource[] wallet =
        {
            new Resource(_bcAmount, ResourceType.BananaCoins),
            new Resource(_bpAmount, ResourceType.BluePrints),
            new Resource(_sfAmount, ResourceType.StarFruits)
        };

        private FirebaseDatabase _database;
        private DatabaseReference _userData;
        
        public FirebaseUser firebaseUser;
        public static bool ConsentGiven =>
            PlayerPrefs.HasKey(Constants.ConsentKey) && PlayerPrefs.GetInt(Constants.ConsentKey) == 1;
        
        //for testing
        private static long _bcAmount = 100;
        private static long _bpAmount = 10;
        private static long _sfAmount = 1;

        private void StartDatabaseListeners()
        {
            _database = FirebaseDatabase.DefaultInstance;
            _userData = _database.GetReference(Constants.PlayerDataPrefix).Child(firebaseUser.UserId)
                .Child(Constants.PlayerDataSuffix);
            
            _userData.Child(Constants.ChestData).ValueChanged += OnChestDataChanged;
            _userData.Child(Constants.RollData).ValueChanged += OnRollDataChanged;
            _userData.Child(Constants.UpgradeData).ValueChanged += OnUpgradeDataChanged;
            _userData.Child(Constants.WalletData).ValueChanged += OnWalletDataChanged;
        }

        public void StopDatabaseListeners()
        {
            if (firebaseUser == null || firebaseUser.UserId == string.Empty || _userData == null) return;

            _userData.Child(Constants.ChestData).ValueChanged -= OnChestDataChanged;
            _userData.Child(Constants.RollData).ValueChanged -= OnRollDataChanged;
            _userData.Child(Constants.UpgradeData).ValueChanged -= OnUpgradeDataChanged;
            _userData.Child(Constants.WalletData).ValueChanged -= OnWalletDataChanged;
        }

        private void OnDisable()
        {
            StopDatabaseListeners();
        }

        private void OnRollDataChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender != null)
                AlertMessage.Init(sender.ToString());

            if (args.DatabaseError != null)
            {
                AlertMessage.Init(args.DatabaseError.Message);
                return;
            }

            if (string.IsNullOrEmpty(args.Snapshot.GetRawJsonValue()))
            {
                AlertMessage.Init("Empty Snapshot: " + args.Snapshot.Key);
                return;
            }

            var snapReturn = args.Snapshot.GetRawJsonValue();
            
            var snapReturnDto = new RollDataDto(snapReturn);
            lastResult = snapReturnDto.lr.ToArray();
            nextResult = snapReturnDto.nr.ToArray();
            currentChestRoll = snapReturnDto.cr;
            EventManager.refreshUi.Raise();
        }

        private void OnChestDataChanged(object sender, ValueChangedEventArgs args)
        {
            chestData = new GenericArrayDto(ProcessDataChanges(sender, args)).newDataArray;
            EventManager.chestRefresh.Raise();
        }

        private void OnWalletDataChanged(object sender, ValueChangedEventArgs args)
        {
            var walletData = new GenericArrayDto(ProcessDataChanges(sender, args)).newDataArray;
            for (var i = 0; i < wallet.Length; i++)
            {
                wallet[i].resourceAmount = walletData[i];
            }
        }

        private void OnUpgradeDataChanged(object sender, ValueChangedEventArgs args)
        {
            upgradeData = new GenericArrayDto(ProcessDataChanges(sender, args)).newDataArray;
        }

        private static string ProcessDataChanges(object sender, ValueChangedEventArgs args)
        {
            if (sender != null)
                AlertMessage.Init(sender.ToString());

            if (args.DatabaseError != null)
            {
                AlertMessage.Init(args.DatabaseError.Message);
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(args.Snapshot.GetRawJsonValue()))
            {
                return args.Snapshot.GetRawJsonValue();
            }

            AlertMessage.Init("Empty Snapshot: " + args.Snapshot.Key);
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

        #region WalletData

        public void DeductCoin(int betAmount)
        {
            wallet[0].resourceAmount -= betAmount;
            EventManager.refreshCurrency.Raise();
        }

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