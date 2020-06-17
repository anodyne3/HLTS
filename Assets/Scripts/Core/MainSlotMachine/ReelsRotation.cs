using DG.Tweening;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ReelsRotation : GlobalAccess
    {
        [SerializeField] private Transform fruitReelL;
        [SerializeField] private Transform fruitReelM;
        [SerializeField] private Transform fruitReelR;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        private int[] _newRoll;
        
        private const float ReelSpinTimeL = 5.666f;
        private const float ReelSpinTimeM = 3.666f;
        private const float ReelSpinTimeR = 2.666f;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, WheelRoll);
            SetReelStartingPosition();
        }

        private void SetReelStartingPosition()
        {
            _newRoll = PlayerData.lastResult;
            if (_newRoll == null)
            {
                AlertMessage.Init("lastResult was empty");
                PlayerData.lastResult = new[] {1, 2, 3};
                _newRoll = PlayerData.lastResult;
                PlayerData.nextResult = new[] {1, 2, 3};
            }

            fruitReelL.Rotate(_newRoll[0] * 30, 0.0f, 0.0f);
            fruitReelM.Rotate(_newRoll[1] * 30, 0.0f, 0.0f);
            fruitReelR.Rotate(_newRoll[2] * 30, 0.0f, 0.0f);
            
            EventManager.testEvent.Raise();
        }

        private void WheelRoll()
        {
            _newRoll = PlayerData.nextResult;

            SpinReels(_newRoll, PlayerData.lastResult);
        }

        
        private void SpinReels(int[] nextRoll, int[] lastResult)
        {
            var spinSequence = DOTween.Sequence();
            
            var spinDegreesL =
                (Constants.TotalSpinTime - 10 - (nextRoll[0] - lastResult[0]) * 2 - Constants.RightReelStopTime) *
                -Constants.FastSpinDegrees
                + (Constants.RightReelStopTime - Constants.MiddleReelStopTime) * -Constants.MediumSpinDegrees
                + Constants.MiddleReelStopTime * -Constants.SlowSpinDegrees;
            var reelFinishRotationL = new Vector3(spinDegreesL, 0.0f, 0.0f);
            
            var spinDegreesM =
                (Constants.TotalSpinTime - 2 + (nextRoll[1] - lastResult[1]) * 2 - Constants.RightReelStopTime) *
                Constants.FastSpinDegrees
                + (Constants.RightReelStopTime - Constants.MiddleReelStopTime) * Constants.MediumSpinDegrees;
            var reelFinishRotationM = new Vector3(spinDegreesM, 0.0f, 0.0f);
            
            var spinDegreesR =
                (Constants.TotalSpinTime - 6 - (nextRoll[2] - lastResult[2]) * 2 - Constants.RightReelStopTime) *
                -Constants.FastSpinDegrees;
            var reelFinishRotationR = new Vector3(spinDegreesR, 0.0f, 0.0f);

            spinSequence.Insert(0.0f,
                    fruitReelL.transform.DOLocalRotate(reelFinishRotationL, ReelSpinTimeL, RotateMode.LocalAxisAdd)
                        .SetEase(curve))
                .Insert(0.0f,
                    fruitReelM.transform.DOLocalRotate(reelFinishRotationM, ReelSpinTimeM, RotateMode.LocalAxisAdd)
                        .SetEase(curve))
                .Insert(0.0f,
                    fruitReelR.transform.DOLocalRotate(reelFinishRotationR, ReelSpinTimeR, RotateMode.LocalAxisAdd)
                        .SetEase(curve))
                .OnComplete(GenerateResult);
        }

        private void GenerateResult()
        {
            if (SlotMachine.result == null)
                SlotMachine.result = new int[3];

            SlotMachine.result[0] = _newRoll[0];
            SlotMachine.result[1] = _newRoll[1];
            SlotMachine.result[2] = _newRoll[2];

            EventManager.wheelResult.Raise();
        }
    }
}