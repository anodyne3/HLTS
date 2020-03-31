using System;
using UnityEngine;

namespace Core.Managers
{
    public class InputManager : GlobalAccess
    {
        protected void Dragging()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnDragBegin(mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                var mousePos = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
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

        protected virtual void OnDragBegin(Vector2 inputPos)
        {
            
        }

        protected virtual void OnDragging(Vector2 inputPos)
        {
            
        }
        
        public virtual void OnDragEnd()
        {
            CameraManager.draggingDisabled = false;
        }
    }
}