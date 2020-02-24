using System.Collections;
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

        private Vector3 _fruitReelLRotation;
        private Vector3 _fruitReelMRotation;
        private Vector3 _fruitReelRRotation;

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.wheelRollEvent, WheelRoll);
            RandomizeReels();
        }

        private void WheelRoll()
        {
            StartCoroutine(nameof(SpinReel));
        }

        private void RandomizeReels()
        {
            _fruitReelLRotation = fruitReelL.rotation.eulerAngles;
            _fruitReelLRotation.x += Random.Range(0, 12) * 30.0f;
            fruitReelL.rotation = Quaternion.Euler(_fruitReelLRotation);

            _fruitReelMRotation = fruitReelM.rotation.eulerAngles;
            _fruitReelMRotation.x -= Random.Range(0, 12) * 30.0f;
            fruitReelM.rotation = Quaternion.Euler(_fruitReelMRotation);

            _fruitReelRRotation = fruitReelR.rotation.eulerAngles;
            _fruitReelRRotation.x += Random.Range(0, 12) * 30.0f;
            fruitReelR.rotation = Quaternion.Euler(_fruitReelRRotation);
        }

        private IEnumerator SpinReel()
        {
            var spinDegrees = Constants.TotalSpinTime;

            _fruitReelLRotation = fruitReelL.rotation.eulerAngles;
            _fruitReelMRotation = fruitReelM.rotation.eulerAngles;
            _fruitReelRRotation = fruitReelR.rotation.eulerAngles;

            while (spinDegrees > Constants.RightReelStopTime)
            {
                spinDegrees -= 1;

                _fruitReelLRotation.x += Constants.FastSpinDegrees;
                fruitReelL.rotation = Quaternion.Euler(_fruitReelLRotation);

                _fruitReelMRotation.x -= Constants.FastSpinDegrees;
                fruitReelM.rotation = Quaternion.Euler(_fruitReelMRotation);

                _fruitReelRRotation.x += spinDegrees - 7 <= Constants.RightReelStopTime
                    ? Constants.FastSpinDegrees * 0.25f
                    : Constants.FastSpinDegrees;
                fruitReelR.rotation = Quaternion.Euler(_fruitReelRRotation);

                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > Constants.MiddleReelStopTime)
            {
                spinDegrees -= 1;

                _fruitReelLRotation.x += Constants.MediumSpinDegrees;
                fruitReelL.rotation = Quaternion.Euler(_fruitReelLRotation);

                _fruitReelMRotation.x -= spinDegrees - 23 <= Constants.MiddleReelStopTime
                    ? Constants.MediumSpinDegrees * 0.25f
                    : Constants.MediumSpinDegrees;
                fruitReelM.rotation = Quaternion.Euler(_fruitReelMRotation);

                yield return new WaitForEndOfFrame();
            }

            while (spinDegrees > 0)
            {
                spinDegrees -= 1;

                _fruitReelLRotation.x += Constants.SlowSpinDegrees;
                fruitReelL.rotation = Quaternion.Euler(_fruitReelLRotation);

                yield return new WaitForEndOfFrame();
            }

            SlotMachine.result = GenerateResult();
            yield return null;
        }

        private int[] GenerateResult()
        {
            return new[]
            {
                ProcessEachResult(fruitReelL.rotation.eulerAngles.x),
                ProcessEachResult(fruitReelM.rotation.eulerAngles.x),
                ProcessEachResult(fruitReelR.rotation.eulerAngles.x),
            };
        }

        private static int ProcessEachResult(float value)
        {
            var remainder = value % 360 / 30;
            return remainder <= 0 ? Mathf.RoundToInt(remainder) + 12 : Mathf.RoundToInt(remainder);
        }
    }
}