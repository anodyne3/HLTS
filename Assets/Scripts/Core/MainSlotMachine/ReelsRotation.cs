using System.Collections;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ReelsRotation : GlobalAccess
    {
        [SerializeField] public Transform fruitReelL;
        [SerializeField] public Transform fruitReelM;
        [SerializeField] public Transform fruitReelR;

        //temp newRoll, will come from server
        [SerializeField] private int[] newRoll;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, WheelRoll);
            SetReelStartingPosition();
        }

        private void SetReelStartingPosition()
        {
            newRoll = PlayerData.lastResult;
            if (newRoll == null)
            {
                Debug.Log("lastResult was empty");
                return;
            }
            
            fruitReelL.Rotate(newRoll[0] * 30, 0.0f, 0.0f);
            fruitReelM.Rotate(newRoll[1] * 30, 0.0f, 0.0f);
            fruitReelR.Rotate(newRoll[2] * 30, 0.0f, 0.0f);
        }

        private void WheelRoll()
        {
            newRoll = PlayerData.nextResult;
            
            StartCoroutine(SpinLeftReel(newRoll[0], PlayerData.lastResult[0]));
            StartCoroutine(SpinMiddleReel(newRoll[1], PlayerData.lastResult[1]));
            StartCoroutine(SpinRightReel(newRoll[2], PlayerData.lastResult[2]));
        }

        private IEnumerator SpinLeftReel(int nextRoll, int lastResult)
        {
            var spinDegrees = Constants.TotalSpinTime - 10.0f - (nextRoll - lastResult) * 2;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;
                fruitReelL.Rotate(-Constants.FastSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > Constants.MiddleReelStopTime)
            {
                spinDegrees -= 1;
                fruitReelL.Rotate(-Constants.MediumSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > 0)
            {
                spinDegrees -= 1;
                fruitReelL.Rotate(-Constants.SlowSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }
            
            GenerateResult();
        }

        private IEnumerator SpinMiddleReel(int nextRoll, int lastResult)
        {
            var spinDegrees = Constants.TotalSpinTime + 4.0f + (nextRoll - lastResult) * 2;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;
                fruitReelM.Rotate(Constants.FastSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > Constants.MiddleReelStopTime)
            {
                spinDegrees -= 1;
                fruitReelM.Rotate(
                    spinDegrees - 23 <= Constants.MiddleReelStopTime
                        ? Constants.MediumSpinDegrees * 0.25f
                        : Constants.MediumSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator SpinRightReel(int nextRoll, int lastResult)
        {
            var spinDegrees = Constants.TotalSpinTime - (nextRoll - lastResult) * 2;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;
                fruitReelR.Rotate(
                    spinDegrees - 7 <= Constants.RightReelStopTime
                        ? -Constants.FastSpinDegrees * 0.25f
                        : -Constants.FastSpinDegrees, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();
            }
        }

        private void GenerateResult()
        {
            if (SlotMachine.result == null)
                SlotMachine.result = new int[3];

            SlotMachine.result[0] = newRoll[0];
            SlotMachine.result[1] = newRoll[1];
            SlotMachine.result[2] = newRoll[2];
            
            for (var i = 0; i < SlotMachine.result.Length; i++)
            {
                if (SlotMachine.result[i] > 11)
                    SlotMachine.result[i] -= 12;
                else if (SlotMachine.result[i] < 0)
                    SlotMachine.result[i] += 12;

                if (SlotMachine.result[i] > 11)
                    SlotMachine.result[i] -= 12;
                else if (SlotMachine.result[i] < 0)
                    SlotMachine.result[i] += 12;
            }

            EventManager.wheelResult.Raise();
        }
    }
}