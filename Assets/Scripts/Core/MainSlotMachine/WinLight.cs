using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class WinLight : GlobalAccess
    {
        private Animator _winLightAnimator;

        private void Start()
        {
            _winLightAnimator = (Animator) GetComponent(typeof(Animator));
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, EnableLight);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutFinishEvent, DisableLight);

            DisableLight();
        }

        private void EnableLight()
        {
            _winLightAnimator.speed = 1.0f;
        }

        private void DisableLight()
        {
            _winLightAnimator.speed = 0.0f;
            _winLightAnimator.Play(Constants.WinLightState, 0, 0.0f);
        }
    }
}