using System.Collections;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ArmDragHandler : GlobalAccess
    {
        private float _deltaYStart;
        private bool _armPullUnlocked;
        private Collider2D _armCollider;
        private Animator _pivotAnimator;
        private bool _isDragging;
        private float _playBackTime;
        private float _deltaY;

        private void Start()
        {
            _armCollider = (Collider2D) GetComponent(typeof(Collider2D));
            _pivotAnimator = (Animator) transform.parent.GetComponentInParent(typeof(Animator));

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, UnlockArmPull);

            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
        }

        private void OnDestroy()
        {
            if (InputManager == null) return;

            InputManager.Pressed -= OnPressed;
            InputManager.Dragged -= OnDragged;
            InputManager.Released -= OnReleased;
        }

        private void OnPressed(Vector2 pointerPosition)
        {
            if (GameManager != null && !GameManager.interactionEnabled) return;
            
            if (_armCollider != Physics2D.OverlapPoint(pointerPosition)) return;

            CameraManager.draggingDisabled = true;

            _isDragging = true;
            _deltaYStart = pointerPosition.y;
        }

        private void OnDragged(Vector2 pointerPosition)
        {
            if (!_isDragging) return;

            _deltaY = _deltaYStart - pointerPosition.y > 0.0f ? _deltaYStart - pointerPosition.y : 0.0f;
            if (_deltaY < 0.0f) return;

            _deltaY *= Constants.ArmDragMultiplier;

            CameraManager.draggingDisabled = true;
            _playBackTime = _deltaY;
            _pivotAnimator.Play(Constants.ArmPullState, 0, _playBackTime);
            _pivotAnimator.speed = 0.0f;

            if (_deltaY > Constants.ArmLockedTriggerAmount && (!_armPullUnlocked || SlotMachine.wheelsAreRolling))
            {
                Release();
                return;
            }

            if (!(_deltaY > Constants.ArmPullTriggerAmount) || !_armPullUnlocked) return;

            _pivotAnimator.Play(Constants.ArmPullState, 0, Constants.ClipTriggerTime);
            _pivotAnimator.speed = 1.0f;
            EventManager.armPull.Raise();
            _isDragging = false;
            _deltaY = 0;
            _armPullUnlocked = false;
        }

        private void OnReleased()
        {
            Release();

            CameraManager.draggingDisabled = false;
        }

        private void Release()
        {
            if (_isDragging)
                StartCoroutine(nameof(ResetArmPullToRest));

            _isDragging = false;
        }

        private void UnlockArmPull()
        {
            _armPullUnlocked = true;
        }

        private IEnumerator ResetArmPullToRest()
        {
            while (_deltaY > 0)
            {
                _deltaY -= Time.deltaTime * Constants.ArmPullResetSpeed;
                _playBackTime = _deltaY;
                _pivotAnimator.Play(Constants.ArmPullState, 0, _playBackTime);
                _pivotAnimator.speed = 0.0f;
                yield return null;
            }
        }
    }
}