using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    public class ReelsRotation : GlobalAccess
    {
        [SerializeField] public Transform fruitReelL;
        [SerializeField] public Transform fruitReelM;
        [SerializeField] public Transform fruitReelR;
        
        //temp newRoll
        private int[] newRoll = {0, 0, 0};

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, WheelRoll);
            //var newRoll = new[] {Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12)};
            RandomizeReels(newRoll);
        }

        private void RandomizeReels(IReadOnlyList<int> newRoll)
        {
            fruitReelL.Rotate(newRoll[0] * 30, 0.0f, 0.0f);
            fruitReelM.Rotate(newRoll[1] * 30, 0.0f, 0.0f);
            fruitReelR.Rotate(newRoll[2] * 30, 0.0f, 0.0f);
        }

        private void WheelRoll()
        {
            //var newRoll = new[] {Random.Range(0, 12), Random.Range(0, 12), Random.Range(0, 12)};
            StartCoroutine(SpinReel(newRoll));
        }

        private IEnumerator SpinReel(IReadOnlyList<int> newRoll)
        {
            var spinDegrees = Constants.TotalSpinTime + newRoll[0] * 12;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;

                fruitReelL.Rotate(-Constants.FastSpinDegrees + newRoll[0] * 2, 0.0f, 0.0f);

                fruitReelM.Rotate(Constants.FastSpinDegrees + newRoll[1] * 2, 0.0f, 0.0f);

                fruitReelR.Rotate(
                    spinDegrees - 7 <= Constants.RightReelStopTime + newRoll[2] * 2
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

            SlotMachine.result = GenerateResult();
            EventManager.wheelResult.Raise();
            yield return null;
        }

        private int[] GenerateResult()
        {
            return new[]
            {
                newRoll[0],
                newRoll[1],
                newRoll[2]
            };
            /*{
                ProcessEachResult(fruitReelL.rotation.eulerAngles.x),
                ProcessEachResult(fruitReelM.rotation.eulerAngles.x),
                ProcessEachResult(fruitReelR.rotation.eulerAngles.x),
            };*/
        }

        private static int ProcessEachResult(float value)
        {
            var result = Mathf.RoundToInt(value) / 30;
            return result <= 0 ? result + 12 : result;
        }
    }
}