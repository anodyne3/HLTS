using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.MainSlotMachine;
using Enums;
using UnityEngine;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        private bool _armIsPulled;
        private Coroutine _autoRoll;
        [HideInInspector] public int lastBetAmount;

        private bool CoinIsLoaded => BetAmount > 0;
        public bool CoinSlotFull => BetAmount >= CoinSlotMaxBet;

        private static int CoinSlotMaxBet => UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.CoinSlot) + 1;

        [HideInInspector] public bool wheelsAreRolling;
        [HideInInspector] public int[] result = new int[3];
        [HideInInspector] public List<FruitType> payoutType = new List<FruitType>();
        [HideInInspector] public long payoutAmount;
        [HideInInspector] public bool autoMode;
        [HideInInspector] public int coinsWereLoaded;

        private int _betAmount;

        public int BetAmount
        {
            get => _betAmount;
            private set
            {
                if (value <= CoinSlotMaxBet && value >= 0)
                    _betAmount = value;

                BetIndicator.RefreshBetIndicators();
            }
        }

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, LoadCoin);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, PullArm);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelResultEvent, WheelResult);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.autoRollEvent, AutoSlotToggle);
        }

        private void LoadCoin()
        {
            BetAmount += 1;
            BetIndicator.RefreshLastBetIndicators();
        }

        private void PullArm()
        {
            if (_armIsPulled || !CoinIsLoaded) return;

            _armIsPulled = true;
            coinsWereLoaded = 0;
            ConsumeCoins();
            OnWheelRoll();
        }

        private void ConsumeCoins()
        {
            PlayerData.DeductCoin(BetAmount);
            lastBetAmount = BetAmount;
            BetIndicator.RefreshLastBetIndicators();
            
            if (!autoMode)
                BetAmount = 0;
        }

        private void OnWheelRoll()
        {
            wheelsAreRolling = true;
            EventManager.wheelRoll.Raise();
            FirebaseFunctionality.RollReels(lastBetAmount);
        }

        private void WheelResult()
        {
            wheelsAreRolling = false;
            DeterminePayout();

            _armIsPulled = false;
        }

        private void DeterminePayout()
        {
            payoutType.Clear();

            for (var i = 0; i < lastBetAmount; i++)
            {
                var fruitResult = new FruitType[3];
                var adjustedResults = AdjustResultsForBet(i);

                var resultLength = result.Length;
                for (var j = 0; j < resultLength; j++)
                    fruitResult[j] = Constants.FruitDefinitions.First(x => x.Id == adjustedResults[j]).FruitType;

                if (fruitResult.Distinct().Count() == 1)
                {
                    payoutType.Add(fruitResult[0]);
                    BetIndicator.FlashLastBetIndicators(i);
                }
                else
                {
                    var fruitGroup = fruitResult.Aggregate(0,
                        (total, next) => next == FruitType.Bananas || next == FruitType.Bars ? total + 1 : total);

                    if (fruitGroup != 3) continue;
                    
                    payoutType.Add(FruitType.Barnana);
                    BetIndicator.FlashLastBetIndicators(i);
                }
            }
            
            BetIndicator.RefreshLastBetIndicators();

            if (payoutType.Count == 0)
                return;

            if (autoMode)
            {
                autoMode = false;
                StopCoroutine(_autoRoll);
                EventManager.refreshUi.Raise();
            }

            BetAmount = coinsWereLoaded;
            CurrencyManager.blockCurrencyRefresh = true;
            EventManager.payoutStart.Raise();
        }

        private int[] AdjustResultsForBet(int betValue)
        {
            var fruitArray = new int[3]; 
            result.CopyTo(fruitArray, 0);
            var resultLength = result.Length;
            switch (betValue)
            {
                case 1:
                    for (var i = 0; i < resultLength; i++)
                        fruitArray[i]++;
                    break;
                case 2:
                    for (var i = 0; i < resultLength; i++)
                        fruitArray[i]--;
                    break;
                case 3:
                    for (var i = 0; i < resultLength; i++)
                        switch (i)
                        {
                            case 0:
                                fruitArray[i]--;
                                break;
                            case 2:
                                fruitArray[i]++;
                                break;
                        }

                    break;
                case 4:
                    for (var i = 0; i < resultLength; i++)
                        switch (i)
                        {
                            case 0:
                                fruitArray[i]++;
                                break;
                            case 2:
                                fruitArray[i]--;
                                break;
                        }

                    break;
            }

            var fruitArrayLength = fruitArray.Length;
            for (var i = 0; i < fruitArrayLength; i++)
            {
                if (fruitArray[i] > 11)
                    fruitArray[i] -= 12;
                else if (fruitArray[i] < 0)
                    fruitArray[i] += 12;

                if (fruitArray[i] > 11)
                    fruitArray[i] -= 12;
                else if (fruitArray[i] < 0)
                    fruitArray[i] += 12;
            }

            return fruitArray;
        }

        private void AutoSlotToggle()
        {
            autoMode = !autoMode;

            if (!autoMode)
            {
                StopCoroutine(_autoRoll);
                BetAmount = 0;
            }
            else
                _autoRoll = StartCoroutine(nameof(AutoMode));

            EventManager.refreshUi.Raise();
        }

        private IEnumerator AutoMode()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitBetweenRolls = new WaitForSeconds(Constants.PauseBetweenRolls);

            if (BetAmount == 0 || UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.AutoRoll) < 2)
                BetAmount = 1;

            _armIsPulled = true;

            while (CurrencyManager.GetCurrencyAmount(ResourceType.BananaCoins) > 0 && autoMode)
            {
                if (wheelsAreRolling)
                {
                    yield return waitUntilWheelsStop;
                    yield return waitBetweenRolls;
                }

                ConsumeCoins();
                OnWheelRoll();

                yield return null;
            }

            yield return null;
        }

        public void BetMin()
        {
            BetAmount = 1;
        }

        public void BetLess()
        {
            if (BetAmount > 1)
                BetAmount--;
        }

        public void BetMore()
        {
            if (BetAmount == 0)
                BetAmount = 2;
            else
                BetAmount++;
        }

        public void BetMax()
        {
            BetAmount = CoinSlotMaxBet;
        }
        
        /*
        //test variables
        private int _testCoinsSpent;
        private float _timeElapsed;
        
        private IEnumerator PayoutRateTest()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitUntilCoinIsLoaded = new WaitUntil(() => CoinIsLoaded);

            var timeStarted = Time.time;

            while (CurrencyManager.GetCurrencyAmount(ResourceType.BananaCoins) > 0 && autoMode)
            {
                if (!CoinIsLoaded)
                    yield return waitUntilCoinIsLoaded;

                if (wheelsAreRolling)
                    yield return waitUntilWheelsStop;

                EventManager.armPull.Raise();

                _timeElapsed = Time.time - timeStarted;
                _testCoinsSpent += 1;
                yield return null;
            }

            autoMode = false;
            yield return null;
        }

        private void OnGUI()
        {
            if (!autoMode) return;
            
            GUI.Box(new Rect(10,10,100,50), "Coins Spent:");
            GUI.Box(new Rect(10,30,100,30), _testCoinsSpent.ToString());
            GUI.Box(new Rect(120, 10, 100, 50), "Test Duration");
            GUI.Box(new Rect(120, 30, 100, 30), _timeElapsed + "s");
        }
        */
    }
}