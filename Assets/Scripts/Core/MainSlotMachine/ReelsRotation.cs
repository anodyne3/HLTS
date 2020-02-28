using System.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core.MainSlotMachine
{
    public class ReelsRotation : GlobalAccess
    {
        [SerializeField] public Transform fruitReelL;
        [SerializeField] public Transform fruitReelM;
        [SerializeField] public Transform fruitReelR;

        //temp newRoll, will come from server
        [SerializeField] private int newRoll;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, WheelRoll);
            RandomizeReels( /*newRoll*/);
        }

        private void RandomizeReels( /*int newRoll*/)
        {
            fruitReelL.Rotate(newRoll * -30, 0.0f, 0.0f);
            fruitReelM.Rotate(newRoll * 90, 0.0f, 0.0f);
            fruitReelR.Rotate(newRoll * -210, 0.0f, 0.0f);
        }

        private void WheelRoll( /*newRoll*/)
        {
            newRoll = Random.Range(-12, 11);
            StartCoroutine(SpinReel(newRoll));
        }

        private IEnumerator SpinReel(int nextRoll)
        {
            var spinDegrees = Constants.TotalSpinTime + 2 * -nextRoll;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;

                fruitReelL.Rotate(-Constants.FastSpinDegrees, 0.0f, 0.0f);

                fruitReelM.Rotate(Constants.FastSpinDegrees, 0.0f, 0.0f);

                fruitReelR.Rotate(
                    spinDegrees - 7 <= Constants.RightReelStopTime
                        ? -Constants.FastSpinDegrees * 0.25f
                        : -Constants.FastSpinDegrees, 0.0f, 0.0f);

                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > Constants.MiddleReelStopTime)
            {
                spinDegrees -= 1;

                fruitReelL.Rotate(-Constants.MediumSpinDegrees, 0.0f, 0.0f);

                fruitReelM.Rotate(
                    spinDegrees - 23 <= Constants.MiddleReelStopTime
                        ? Constants.MediumSpinDegrees * 0.25f
                        : Constants.MediumSpinDegrees, 0.0f, 0.0f);

                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > 0)
            {
                spinDegrees -= 1;

                fruitReelL.Rotate(-Constants.SlowSpinDegrees, 0.0f, 0.0f);

                yield return new WaitForEndOfFrame();
            }

            GenerateResult();
            EventManager.wheelResult.Raise();
            yield return null;
        }

        private void GenerateResult()
        {
            if (SlotMachine.result == null)
                SlotMachine.result = new int[3];
            
            SlotMachine.result[0] += newRoll - 5;
            SlotMachine.result[1] += newRoll - 2;
            SlotMachine.result[2] += newRoll;

            for (var i = 0; i < SlotMachine.result.Length; i++)
            {
                if (SlotMachine.result[i] > 12)
                    SlotMachine.result[i] -= 12;
                else if (SlotMachine.result[i] < 0)
                    SlotMachine.result[i] += 12;
            }
        }
    }
}