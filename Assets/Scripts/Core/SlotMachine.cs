using System.Linq;
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
            if (result.Distinct().Count() == 1)
                Debug.LogError("LINQ says distinct.count == 1");

            var payout = 0;
            
            if (result[0] == result[1] && result[2] == result[1])
                switch (result[0])
                {
                    case 1:
                    case 3:
                    case 6:
                    case 8:
                    case 10:
                        payout = Constants.PlumsPayout;
                        break;
                    case 0:
                    case 4:
                    case 7:
                        payout = Constants.CherriesPayout;
                        break;
                    case 2:
                    case 9:
                        payout = Constants.DiamondsPayout;
                        break;
                    case 5:
                    case 11:
                        payout = Constants.BarnanaPayout;                        
                        break;
                }

            if (result.All(x => x == 5 || x == 11))
            {
                payout = Constants.MixedPayout;
                Debug.LogError("LINQ says mixed payout");
            }
            
            PlayerData.AddPayout(payout);
        }
    }
}