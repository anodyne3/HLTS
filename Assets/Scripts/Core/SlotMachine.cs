using System.Collections;
using MyScriptableObjects;
using UnityEngine;

namespace Core
{
    public class SlotMachine : Singleton<SlotMachine>
    {
        public GameEvent armPull;
        public GameEvent wheelRoll;
        public GameEvent wheelResult;

        private bool _armIsPulled;
        private Coroutine _rollingWheels;
        
        //to constants
        private int _wheelSpinTime;

        public void OnArmPull()
        {
            if (_armIsPulled) return;
            
            _armIsPulled = true;
        }

        public void OnWheelRoll()
        {
            if (_rollingWheels != null) return;

            _rollingWheels = StartCoroutine(RollWheels());
        }

        private IEnumerator RollWheels()
        {
            return null;
        }

        public void OnWheelResult()
        {
            DeterminePayout();
            
            _armIsPulled = false;
        }

        private void DeterminePayout()
        {
            
        }
    }
}