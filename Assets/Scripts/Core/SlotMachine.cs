using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private bool _armIsPulled;

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
            var payout = 0;

            var fruitResult = new FruitType[3];

            for (var i = 0; i < result.Length; i++)
                fruitResult[i] = Constants.FruitDefinitions.First(x => x.Id == result[i]).FruitType;

            if (fruitResult.Distinct().Count() == 1)
            {
                switch (fruitResult[0])
                {
                    case FruitType.Plum:
                        payout = Constants.PlumsPayout;
                        break;
                    case FruitType.Cherries:
                        payout = Constants.CherriesPayout;
                        break;
                    case FruitType.Diamond:
                        payout = Constants.DiamondsPayout;
                        break;
                    case FruitType.Banana:
                    case FruitType.Bar:
                        payout = Constants.BarnanaPayout;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Debug.LogError("LINQ says distinct.count == 1 for " + fruitResult[0]);
            }

            var fruitGroup = fruitResult.Aggregate(0,
                (total, next) => next == FruitType.Banana || next == FruitType.Bar ? total + 1 : total);
            
            if (fruitGroup == 3)
            {
                Debug.LogError("LINQ says mixed payout");

                payout = Constants.BarnanaPayout;
            }

            PlayerData.AddPayout(payout);
        }

        public bool autoMode;

        private void AutoSlotMode()
        {
            if (autoMode) return;

            autoMode = true;
            StartCoroutine(nameof(PayoutRateTest));
        }

        private IEnumerator PayoutRateTest()
        {
            var waitUntil = new WaitUntil(() => wheelsAreRolling == false);

            var timeStarted = Time.time;

            while (PlayerData.coinsAmount > 0)
            {
                if (wheelsAreRolling)
                    yield return waitUntil;

                EventManager.coinInsert.Raise();
                EventManager.armPull.Raise();
                _timeElapsed = Time.time - timeStarted;
                _testCoinsSpent += 1;
                yield return null;
            }

            autoMode = false;
            yield return null;
        }

        private int _testCoinsSpent;
        private float _timeElapsed = 0.0f;

        private void OnGUI()
        {
            if (!autoMode) return;
            
            var newRect = new Rect(10,10,50,70);
            
            GUI.Box(newRect, "Coins Spent:");
            GUI.Box(new Rect(10,30,50,50), _testCoinsSpent.ToString());
            GUI.Box(new Rect(70, 10, 50, 70), "Test Duration");
            GUI.Box(new Rect(70, 10, 50, 70), _timeElapsed.ToString(CultureInfo.InvariantCulture));
        }
    }
}