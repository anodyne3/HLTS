using System.Collections;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        private bool _armIsPulled;
        private int _coinWasLoaded;
        private Coroutine _autoRoll;

        private bool CoinIsLoaded => BetAmount > 0;
        public bool CoinSlotFull => BetAmount >= CoinSlotMaxBet;

        private static int CoinSlotMaxBet => UpgradeManager.GetUpgradeCurrentLevel(UpgradeTypes.CoinSlot) + 1;

        [HideInInspector] public bool wheelsAreRolling;
        [HideInInspector] public int[] result = new int[3];
        [HideInInspector] public FruitType payoutType;
        [HideInInspector] public long payoutAmount;
        [HideInInspector] public bool autoMode;

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

        /*//test variables
        private int _testCoinsSpent;
        private float _timeElapsed;*/

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
        }

        private void PullArm()
        {
            if (_armIsPulled || !CoinIsLoaded) return;

            _armIsPulled = true;
            OnWheelRoll();
            ConsumeCoins();
        }

        private void ConsumeCoins()
        {
            PlayerData.DeductCoin(BetAmount);
            if (!autoMode)
                BetAmount = 0;
        }

        private void OnWheelRoll()
        {
            wheelsAreRolling = true;
            EventManager.wheelRoll.Raise();
            FirebaseFunctionality.RollReels(BetAmount);
        }

        private void WheelResult()
        {
            wheelsAreRolling = false;
            DeterminePayout();

            _armIsPulled = false;
        }

        private void DeterminePayout()
        {
            payoutType = FruitType.None;

            var fruitResult = new FruitType[3];

            for (var i = 0; i < result.Length; i++)
                fruitResult[i] = Constants.FruitDefinitions.First(x => x.Id == result[i]).FruitType;

            if (fruitResult.Distinct().Count() == 1)
                payoutType = fruitResult[0];
            else
            {
                var fruitGroup = fruitResult.Aggregate(0,
                    (total, next) => next == FruitType.Bananas || next == FruitType.Bars ? total + 1 : total);

                if (fruitGroup == 3)
                    payoutType = FruitType.Barnana;
            }

            if (payoutType == FruitType.None)
            {
                /*if (!autoMode)
                    BetAmount = 0;*/
                
                return;
            }

            if (autoMode)
            {
                autoMode = false;
                StopCoroutine(_autoRoll);
                BetAmount = _coinWasLoaded;
                EventManager.refreshUi.Raise();
            }

            CurrencyManager.blockCurrencyRefresh = true;
            EventManager.payoutStart.Raise();
        }

        private void AutoSlotToggle()
        {
            autoMode = !autoMode;

            if (!autoMode)
            {
                StopCoroutine(_autoRoll);
                BetAmount = _coinWasLoaded;
            }
            else
                _autoRoll = StartCoroutine(nameof(AutoMode));

            EventManager.refreshUi.Raise();
        }

        private IEnumerator AutoMode()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitBetweenRolls = new WaitForSeconds(Constants.PauseBetweenRolls);

            _coinWasLoaded = BetAmount;

            if (BetAmount == 0)
                BetAmount = 1;
            
            _armIsPulled = true;

            while (PlayerData.GetResourceAmount(ResourceType.BananaCoins) > 0 && autoMode)
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

        /*private IEnumerator PayoutRateTest()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitUntilCoinIsLoaded = new WaitUntil(() => CoinIsLoaded);

            var timeStarted = Time.time;

            while (PlayerData.GetResourceAmount(ResourceType.BananaCoins) > 0 && autoMode)
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
        }*/

        public void BetMin()
        {
            BetAmount = 1;
        }

        public void BetLess()
        {
            BetAmount--;
        }

        public void BetMore()
        {
            BetAmount++;
        }

        public void BetMax()
        {
            BetAmount = CoinSlotMaxBet;
        }

        /*
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