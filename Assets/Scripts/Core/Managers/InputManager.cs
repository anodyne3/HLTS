using System;
using Core.Input;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Vector2 = UnityEngine.Vector2;

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
        private bool _useTouch = true;

        private bool _isDragging;
        public Vector2 pointerPosition;
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
            
            if (CameraManager == null) return;

            if (drag.contact && !_isDragging)
            {
                _isDragging = true;
                pointerPosition = CameraManager.MainCamera.ScreenToWorldPoint(drag.position);
                startPosition = pointerPosition;
                Pressed?.Invoke(pointerPosition);
            }
            else if (drag.contact && _isDragging)
            {
                pointerPosition = CameraManager.MainCamera.ScreenToWorldPoint(drag.position);
                dragDelta = startPosition - pointerPosition;
                startPosition = pointerPosition;
                Dragged?.Invoke(pointerPosition);
            }
            else
            {
                Released?.Invoke();
                _isDragging = false;
            }
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            //sanity for development - remove on prod i guess
            return;
            var control = context.control;
            var device = control.device;

            var isMouseInput = device is Mouse;

            if (isMouseInput)
                Scrolled?.Invoke(context.ReadValue<float>());
        }

        private void OnPinch(InputAction.CallbackContext context)
        {
            if (Touch.activeTouches.Count < 2) return;

            var device = context.control.device;

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