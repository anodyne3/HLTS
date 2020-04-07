using System;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Core.Managers
{
    public class InputManager : Singleton<InputManager>
    {
        public event Action<Vector2> Pressed;
        public event Action<Vector2> Dragged;
        public event Action Released;
        public event Action<float> Scrolled;
        public event Action<Touch, Touch> Pinched;

        private InputActions _inputActions;

        //maybe set these with #ifs
        private bool _useMouse = true;
        private bool _usePen;
        private bool _useTouch;

        private bool _isDragging;
        public Vector2 startPosition;
        public Vector2 dragDelta;

        private void Awake()
        {
            _inputActions = new InputActions();

            _inputActions.Pointer.point.performed += OnAction;
            _inputActions.Pointer.point.canceled += OnAction;
            _inputActions.Pointer.point.performed += OnPinch;
            _inputActions.Pointer.point.canceled += OnPinch;
            _inputActions.Pointer.scroll.performed += OnScroll;
            _inputActions.Pointer.scroll.canceled += OnScroll;

            SyncBindingMask();
        }

        private void Start()
        {
            EnhancedTouchSupport.Enable();

            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions?.Disable();
        }

        private void OnAction(InputAction.CallbackContext context)
        {
            var drag = context.ReadValue<PointerInput>();

            if (drag.contact && !_isDragging)
            {
                Pressed?.Invoke(CameraManager.MainCamera.ScreenToWorldPoint(drag.position));
                _isDragging = true;
            }
            else if (drag.contact && _isDragging)
            {
                Dragged?.Invoke(CameraManager.MainCamera.ScreenToWorldPoint(drag.position));
            }
            else
            {
                Released?.Invoke();
                _isDragging = false;
            }
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            var control = context.control;
            var device = control.device;

            var isMouseInput = device is Mouse;
            // var isTouchInput = device is Touchscreen;

            if (isMouseInput)
                Scrolled?.Invoke(context.ReadValue<float>());
            // else if (isTouchInput)
            //     Pinched?.Invoke(context.ReadValue<PointerInput>());
        }

        private void OnPinch(InputAction.CallbackContext context)
        {
            if (Touch.activeTouches.Count < 2) return;

            var control = context.control;
            var device = control.device;

            var isTouchInput = device is Touchscreen;

            if (!isTouchInput) return;

            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];

            Pinched?.Invoke(touch0, touch1);
        }

        private void SyncBindingMask()
        {
            if (_inputActions == null)
                return;

            if (_useMouse && _usePen && _useTouch)
            {
                _inputActions.bindingMask = null;
                return;
            }

            _inputActions.bindingMask = InputBinding.MaskByGroups(_useMouse ? "Mouse" : null, _usePen ? "Pen" : null,
                _useTouch ? "Touch" : null);
        }
    }
}