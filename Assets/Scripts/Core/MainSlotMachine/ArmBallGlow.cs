using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ArmBallGlow : GlobalAccess
    {
        private Animator _ballGlowAnimator;

        private void Start()
        {
            _ballGlowAnimator = (Animator) GetComponent(typeof(Animator));
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, EnableGlow);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, DisableGlow);

            DisableGlow();
        }

        private void EnableGlow()
        {
            _ballGlowAnimator.speed = 1.0f;
        }

        private void DisableGlow()
        {
            _ballGlowAnimator.speed = 0.0f;
            _ballGlowAnimator.Play(Constants.LeverBallGlowState, 0, 0.0f);
        }
    }
}