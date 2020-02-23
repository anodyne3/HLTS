using System;
using UnityEngine;
using Utils;

namespace Core
{
    public class ArmDragHandler : GlobalAccess
    {
        private float _deltaYStart;
        [SerializeField] private bool _armPullUnlocked;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _armCollider;
        private Animator _pivotAnimator;
        private bool _isDragging;

        public float deltaY;
        public Vector2 mousePos;

        private void Start()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            _armCollider = (Collider2D) GetComponent(typeof(Collider2D));
            _pivotAnimator = (Animator) GetComponentInParent(typeof(Animator));
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, UnlockArmPull);
        }

        private void FixedUpdate()
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
            //_pivotAnimator.speed = 0;
        }

        public float ArmPullTriggerAmount = 5.0f;
        public float ArmLockedTriggerAmount = 2.0f;
        public float ClipTriggerTime = 0.6f;
        public float playBackTime;

        private void OnDragging(Vector2 inputPos)
        {
            if (!_isDragging) return;

            deltaY = Mathf.Abs(inputPos.y - _deltaYStart);

            playBackTime = ClipTriggerTime * deltaY / ArmPullTriggerAmount;
            _pivotAnimator.Play("LeverPull", 0, playBackTime);

            if (deltaY > ArmLockedTriggerAmount && !_armPullUnlocked)
            {
                Debug.Log("deltaY > ArmLockedTriggerAmount && !_armPullUnlocked");
                //play reversed
                return;
            }

            if (deltaY > ArmPullTriggerAmount && _armPullUnlocked)
            {
                Debug.Log("deltaY > Constants.ArmPullTriggerAmount && _armPullUnlocked");
                _pivotAnimator.Play("LeverPull", 0, ClipTriggerTime);
                _pivotAnimator.speed = 1.0f;
                EventManager.armPull.Raise();
                LockArmPull();
            }
        }

        private void OnDragEnd()
        {
            _isDragging = false;
            deltaY = 0.0f;
        }

        private void UnlockArmPull()
        {
            _armPullUnlocked = true;
        }
        
        private void LockArmPull()
        {
            _armPullUnlocked = false;
        }

        private void SetPullArmLightColor()
        {
            //dependant on coin type, set light color
            //_spriteRenderer.color =   
        }
    }
}