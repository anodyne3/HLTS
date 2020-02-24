using System.Linq;
using Enums;
using UnityEngine;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        public bool coinIsLoaded;
        public int[] result;

        [SerializeField] private bool _armIsPulled;
        [SerializeField] private bool _wheelsAreRolling;

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
            if (_armIsPulled) return;

            if (coinIsLoaded)
            {
                _armIsPulled = true;
                ConsumeCoin();
                OnWheelRoll();
            }
            else
                AudioManager.PlayClip(SoundEffectType.ArmPull); //new sound needed for empty pull
        }

        private void ConsumeCoin()
        {
            coinIsLoaded = false;
            EventManager.coinConsume.Raise();
        }

        private void OnWheelRoll()
        {
            _wheelsAreRolling = true;
            EventManager.wheelRoll.Raise();
        }

        private void WheelResult()
        {
            _wheelsAreRolling = false;
            DeterminePayout();

            _armIsPulled = false;
        }

        private void DeterminePayout()
        {
            if (result.All(x => true))
                Debug.LogError("LINQ says true");
            
            if (result[0] == result[1] && result[1] == result[2])
                switch (result[0])
                {
                    case 1:
                    case 3:
                    case 6:
                    case 8:
                    case 10:
                        PlayerData.coinsAmount += Constants.PlumsPayout;
                        break;
                    case 0:
                    case 4:
                    case 7:
                        PlayerData.coinsAmount += Constants.CherriesPayout;
                        break;
                    case 2:
                    case 9:
                        PlayerData.coinsAmount += Constants.DiamondsPayout;
                        break;
                    case 5:
                    case 11:
                        PlayerData.coinsAmount += Constants.BarnanaPayout;
                        break;
                }

            if (result.All(x => x == 5 || x == 11))
            {
                PlayerData.coinsAmount += Constants.MixedPayout;
                Debug.LogError("LINQ says mixed payout");
            }
            
            EventManager.wheelResult.Raise();
        }
    }
}