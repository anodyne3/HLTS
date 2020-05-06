using System.Collections;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        [HideInInspector] public bool coinIsLoaded;
        [HideInInspector] public bool wheelsAreRolling;
        [HideInInspector] public int[] result = new int[3];
        [HideInInspector] public FruitType payout;
        [HideInInspector] public bool autoMode;
        [HideInInspector] public int betAmount;
        [HideInInspector] public int coinSlotMaxBet;
 
        private bool _armIsPulled;
        private bool _coinWasLoaded;
        private Coroutine _autoRoll;
        
        //test variables
        private int _testCoinsSpent;
        private float _timeElapsed;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, LoadCoin);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, PullArm);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelResultEvent, WheelResult);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.autoRollEvent, AutoSlotToggle);
        }

        private void LoadCoin()
        {
            if (coinIsLoaded) return;

            coinIsLoaded = true;
        }

        private void PullArm()
        {
            if (_armIsPulled || !coinIsLoaded) return;

            _armIsPulled = true;
            ConsumeCoin();
            OnWheelRoll();
        }

        private void ConsumeCoin()
        {
            coinIsLoaded = false;
            EventManager.coinConsume.Raise();
        }

        private void OnWheelRoll()
        {
            wheelsAreRolling = true;
            EventManager.wheelRoll.Raise();
        }

        private void WheelResult()
        {
            wheelsAreRolling = false;
            DeterminePayout();

            _armIsPulled = false;
        }

        private void DeterminePayout()
        {
            payout = FruitType.None;
            
            var fruitResult = new FruitType[3];

            for (var i = 0; i < result.Length; i++)
                fruitResult[i] = Constants.FruitDefinitions.First(x => x.Id == result[i]).FruitType;

            if (fruitResult.Distinct().Count() == 1)
                payout = fruitResult[0];
            else
            {
                var fruitGroup = fruitResult.Aggregate(0,
                    (total, next) => next == FruitType.Bananas || next == FruitType.Bars ? total + 1 : total);

                if (fruitGroup == 3)
                    payout = FruitType.Barnana;
            }

            if (payout == FruitType.None) return;

            if (autoMode)
            {
                autoMode = false;
                coinIsLoaded = _coinWasLoaded;
                StopCoroutine(_autoRoll);
                EventManager.refreshUi.Raise();
            }

            EventManager.payoutStart.Raise();
        }

        private void AutoSlotToggle()
        {
            autoMode = !autoMode;
            
            if (!autoMode) return;
            _autoRoll = StartCoroutine(nameof(AutoMode));
        }

        private IEnumerator AutoMode()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);

            _coinWasLoaded = coinIsLoaded;
            coinIsLoaded = true;
            _armIsPulled = true;
            
            while (PlayerData.GetResourceAmount(ResourceType.BananaCoins) > 0 && autoMode)
            {
                if (wheelsAreRolling)
                    yield return waitUntilWheelsStop;

                EventManager.coinConsume.Raise();
                OnWheelRoll();

                yield return null;
            }
            yield return null;
        }

        private IEnumerator PayoutRateTest()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitUntilCoinIsLoaded = new WaitUntil(() => coinIsLoaded);

            var timeStarted = Time.time;

            while (PlayerData.GetResourceAmount(ResourceType.BananaCoins) > 0 && autoMode)
            {
                EventManager.coinInsert.Raise();

                if (!coinIsLoaded)
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

        public void BetMin()
        {
            betAmount = 1;
        }

        public void BetLess()
        {
            betAmount--;
        }

        public void BetMore()
        {
            betAmount++;
        }

        public void BetMax()
        {
            betAmount = UpgradeManager.GetUpgradeCurrentLevel(2);
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