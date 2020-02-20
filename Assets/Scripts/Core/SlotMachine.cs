using System.Collections;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>, IDragHandler
    {
        public GameEvent armPull;
        public GameEvent coinLoad;
        public GameEvent wheelResult;
        public GameEvent wheelRoll;

        private Collider _coinSlotCollider;
        private Collider _pullArmCollider;
        
        [SerializeField] private bool _coinIsLoaded;
        [SerializeField] private bool _armIsPulled;
        [SerializeField] private bool _wheelsAreRolling;
        
        //to constants
        private float _wheelSpinTime = 2.0f;

        private void OnEnable()
        {
            armPull = Resources.Load<GameEvent>("Events/armPullEvent");
            coinLoad = Resources.Load<GameEvent>("Events/coinLoadEvent");
            wheelResult = Resources.Load<GameEvent>("Events/wheelResultEvent");
            wheelRoll = Resources.Load<GameEvent>("Events/wheelRollEvent");
        }

        private void OnCollisionStay(Collision other)
        {
            if (_coinIsLoaded) return;

            if (!other.collider.TryGetComponent(out CoinObject coinObject)) return;
            
            //room for a load anim or something where the player loses control of the coin and it drops into the slot
            coinObject.LoadCoinIntoSlot();
            
            _coinIsLoaded = true;
            coinLoad.Raise();
        }

        public void LoadCoin()
        {
            if (_coinIsLoaded) return;

            _coinIsLoaded = true;
            coinLoad.Raise();
        }

        public void PullArm()
        {
            if (_armIsPulled) return;

            if (_coinIsLoaded)
            {
                _armIsPulled = true;
                armPull.Raise();
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
            _coinIsLoaded = false;
            //need new event coinConsumed.Raise();
        }

        private void OnWheelRoll()
        {
            StartCoroutine(RollWheels());

            wheelRoll.Raise();
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
            wheelResult.Raise();
        }

        private void DeterminePayout()
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerPressRaycast.gameObject.GetComponent(typeof(Collider)) == _pullArmCollider)
            {
                if (eventData.delta.y > Constants.ArmPullTriggerAmount)
                    PullArm();
            }
        }
    }
}