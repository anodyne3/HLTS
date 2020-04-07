using System;
using System.Collections;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
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

            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
#if UNITY_EDITOR            
            // InputManager.inputActions.UI.PointerPosition.performed += drag => OnDragBegin(drag.ReadValue<Vector2>());
            // InputManager._inputActions.UI.Click.started += OnDragBegin;
            // InputManager._inputActions.UI.Click.performed += OnDragging;
            // InputManager._inputActions.UI.Click.canceled += _ => OnDragEnd();
            // InputManager.inputActions.UI.Release.performed += OnDragEnd;
#elif UNITY_ANDROID
            // Touch.onFingerDown += OnDragBegin();
            // Touch.onFingerUp += OnDragEnd;
#endif
        }
        
        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (InputManager == null) return;
            
            InputManager.Pressed -= OnPressed;
            InputManager.Dragged -= OnDragged;
            InputManager.Released -= OnReleased;
            // InputManager._inputActions.Pointer.PointerPosition.started -= OnDragBegin;
            // InputManager._inputActions.Pointer.Click.performed -= OnDragging;
            // InputManager.inputActions.UI.Click.canceled -= OnDragEnd;
            // InputManager._inputActions.UI.Release.performed -= _ => OnDragEnd();
#elif UNITY_ANDROID
            // Touch.onFingerDown -= OnDragBegin();
            // Touch.onFingerUp -= OnDragEnd;1
#endif
        }

        private void FixedUpdate()
        {
            // Dragging();
            /*
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
            */
        }
        
        private void OnPressed(PointerInput pointerInput)
        {
            var worldPosition = CameraManager.MainCamera.ScreenToWorldPoint(pointerInput.position);
            if (_armCollider != Physics2D.OverlapPoint(worldPosition)) return;
            
            CameraManager.draggingDisabled = true;

            _isDragging = true;
            _deltaYStart = pointerInput.position.y;
        }
        
        private void OnDragged(PointerInput pointerInput)
        {
            if (!_isDragging) return;    
            
            deltaY = _deltaYStart - pointerInput.position.y > 0.0f ? _deltaYStart - pointerInput.position.y : 0.0f;
            if (deltaY < 0.0f) return;

            deltaY *= InputManager.armDragMultiplier;

            CameraManager.draggingDisabled = true;
            playBackTime = /*Constants.ClipTriggerTime * */deltaY / Constants.ArmPullTriggerAmount;
            _pivotAnimator.Play(Constants.ArmPullState, 0, playBackTime);
            _pivotAnimator.speed = 0.0f;

            if (deltaY > Constants.ArmLockedTriggerAmount && (!_armPullUnlocked || SlotMachine.wheelsAreRolling))
            {
                Release();
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
        
        private void OnReleased(PointerInput pointerInput)
        {
            Release();
            
            CameraManager.draggingDisabled = false;
        }

        private void Release()
        {
            _isDragging = false;
            
            StartCoroutine(nameof(ResetArmPullToRest));
        }

        private void OnDragBegin(InputAction.CallbackContext context)
        // protected void OnDragBegin(Vector2 inputPos)
        // protected override void OnDragBegin(Vector2 inputPos)
        {
            // base.OnDragBegin(inputPos);
            var position = Vector2.one;
            
            if (_armCollider != Physics2D.OverlapPoint(position)) return;
            // if (_armCollider != Physics2D.OverlapPoint(inputPos)) return;
            
            CameraManager.draggingDisabled = true;

            // _isDragging = true;
            _deltaYStart = position.y;
        }

        private void OnDragging(InputAction.CallbackContext context)
        // protected override void OnDragging(Vector2 inputPos, Vector2 inputDelta)
        {
            // base.OnDragging(inputPos, inputDelta);
            // if (!_isDragging) return;    
            
            // deltaY = Touch.activeTouches[0].delta.y;
            // if (Touch.activeTouches[0].delta.y < 0.0f) return;

            deltaY = context.ReadValue<Vector2>().y;
            if (deltaY < 0.0f) return;
            // deltaY = _deltaYStart - inputPos.y > 0.0f ? _deltaYStart - inputPos.y : 0.0f;

            CameraManager.draggingDisabled = true;
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
            // _isDragging = false;
            deltaY = 0;
            _armPullUnlocked = false;
        }

        private void OnDragEnd()
        // public override void OnDragEnd()
        {
            // base.OnDragEnd();

            // _isDragging = false;
            
            CameraManager.draggingDisabled = false;
            
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