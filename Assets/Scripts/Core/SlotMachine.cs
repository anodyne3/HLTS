using System.Collections;
using Core.Managers;
using Enums;
using UnityEngine;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        public bool coinIsLoaded;
        [SerializeField] private bool _armIsPulled;
        [SerializeField] private bool _wheelsAreRolling;
        
        //to constants
        private float _wheelSpinTime = 2.0f;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, LoadCoin);
        }

        public void LoadCoin()
        {
            if (coinIsLoaded) return;

            coinIsLoaded = true;
        }

        public void PullArm()
        {
            if (_armIsPulled) return;

            if (coinIsLoaded)
            {
                _armIsPulled = true;
                EventManager.armPull.Raise();
                ConsumeCoin();
                OnWheelRoll();
            }
            else
            {
                AudioManager.PlayClip(SoundEffectType.ArmPull); //new sound needed for empty pull
            }
            
        }

        private void ConsumeCoin()
        {
            coinIsLoaded = false;
            //need new event coinConsumed.Raise();
        }

        private void OnWheelRoll()
        {
            StartCoroutine(RollWheels());

            EventManager.wheelRoll.Raise();
        }

        private IEnumerator RollWheels()
        {
            _wheelsAreRolling = true;
            
            var t = _wheelSpinTime;
            while (t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            WheelResult();
            yield return null;
        }

        private void WheelResult()
        {
            _wheelsAreRolling = false;
            DeterminePayout();
            
            _armIsPulled = false;
            EventManager.wheelResult.Raise();
        }

        private void DeterminePayout()
        {
            
        }
    }
}