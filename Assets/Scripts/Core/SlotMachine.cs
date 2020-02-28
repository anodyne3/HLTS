using System;
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
        public int[] result;

        private bool _armIsPulled;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, LoadCoin);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, PullArm);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelResultEvent, WheelResult);
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

            if (fruitResult.GroupBy(x => new {FruitType.Banana, FruitType.Bar}).Count() == 3)
            {
                Debug.LogError("LINQ says mixed payout");
                
                payout = Constants.MixedPayout;
            }

            PlayerData.AddPayout(payout);
        }
    }
}