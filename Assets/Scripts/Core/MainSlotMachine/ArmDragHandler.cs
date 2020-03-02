using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ArmDragHandler : GlobalAccess
    {
        private float _deltaYStart;
        private bool _armPullUnlocked;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _armCollider;
        private Animator _pivotAnimator;
        private bool _isDragging;
        public float playBackTime;

        public float deltaY;
        public Vector2 mousePos;

        private void Start()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            _armCollider = (Collider2D) GetComponent(typeof(Collider2D));
            _pivotAnimator = (Animator) transform.parent.GetComponentInParent(typeof(Animator));

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, UnlockArmPull);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                mousePos = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnDragBegin(mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                mousePos = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnDragging(mousePos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnDragEnd();
            }
#endif

            if (Input.touchCount <= 0) return;

            var touch = Input.GetTouch(0);
            Vector2 touchPos = CameraManager.MainCamera.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnDragBegin(touchPos);
                    break;
                case TouchPhase.Moved:
                    OnDragging(touchPos);
                    break;
                case TouchPhase.Ended:
                    OnDragEnd();
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDragBegin(Vector2 inputPos)
        {
            if (_armCollider != Physics2D.OverlapPoint(inputPos)) return;

            _isDragging = true;
            _deltaYStart = inputPos.y;
        }

        private void OnDragging(Vector2 inputPos)
        {
            if (!_isDragging) return;

            deltaY = _deltaYStart - inputPos.y > 0.0f ? _deltaYStart - inputPos.y : 0.0f;

            playBackTime = Constants.ClipTriggerTime * deltaY / Constants.ArmPullTriggerAmount;
            _pivotAnimator.Play(Constants.ArmPullState, 0, playBackTime);
            _pivotAnimator.speed = 0.0f;

            if (deltaY > Constants.ArmLockedTriggerAmount && (!_armPullUnlocked || SlotMachine.wheelsAreRolling))
            {
                OnDragEnd();
                return;
            }

            if (!(deltaY > Constants.ArmPullTriggerAmount) || !_armPullUnlocked) return;
            
            _pivotAnimator.Play(Constants.ArmPullState, 0, Constants.ClipTriggerTime);
            _pivotAnimator.speed = 1.0f;
            EventManager.armPull.Raise();
            _isDragging = false;
            deltaY = 0;
            _armPullUnlocked = false;
        }

        private void OnDragEnd()
        {
            _isDragging = false;
            
            StartCoroutine(nameof(ResetArmPullToRest));
        }

        private void UnlockArmPull()
        {
            _armPullUnlocked = true;
        }

        private IEnumerator ResetArmPullToRest()
        {
            while (deltaY > 0)
            {
                deltaY -= Time.deltaTime * Constants.ArmPullResetSpeed;
                playBackTime = Constants.ClipTriggerTime * deltaY / Constants.ArmPullTriggerAmount;
                _pivotAnimator.Play(Constants.ArmPullState, 0, playBackTime);
                _pivotAnimator.speed = 0.0f;
                yield return null;
            }
        }
    }
}