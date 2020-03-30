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
 
        private bool _armIsPulled;
        
        //test variables
        [HideInInspector] public bool autoMode;
        private int _testCoinsSpent;
        private float _timeElapsed;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, LoadCoin);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, PullArm);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelResultEvent, WheelResult);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.autoSlotModeEvent, AutoSlotMode);
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
            {
                payout = fruitResult[0];
            }
            else
            {
                var fruitGroup = fruitResult.Aggregate(0,
                    (total, next) => next == FruitType.Bananas || next == FruitType.Bars ? total + 1 : total);

                if (fruitGroup == 3)
                {
                    payout = FruitType.Barnana;
                }
            }

            if (payout != FruitType.None)
                EventManager.payoutStart.Raise();
        }

        private void AutoSlotMode()
        {
            if (autoMode) return;

            autoMode = true;
            StartCoroutine(nameof(PayoutRateTest));
        }

        private IEnumerator PayoutRateTest()
        {
            var waitUntilWheelsStop = new WaitUntil(() => wheelsAreRolling == false);
            var waitUntilCoinIsLoaded = new WaitUntil(() => coinIsLoaded);

            var timeStarted = Time.time;

            while (PlayerData.coinsAmount > 0)
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

        private void OnGUI()
        {
            if (!autoMode) return;
            
            GUI.Box(new Rect(10,10,100,50), "Coins Spent:");
            GUI.Box(new Rect(10,30,100,30), _testCoinsSpent.ToString());
            GUI.Box(new Rect(120, 10, 100, 50), "Test Duration");
            GUI.Box(new Rect(120, 30, 100, 30), _timeElapsed + "s");
        }
    }
}