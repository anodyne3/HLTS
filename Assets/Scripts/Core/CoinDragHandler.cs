using System;
using UnityEngine;

namespace Core
{
    public class CoinDragHandler : GlobalAccess
    {
        private float _deltaX, _deltaY;
        private Rigidbody2D _rb;
        private bool _moveAllowed;

        public Vector2 mousePos;

        private void Start()
        {
            _rb = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
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
            if (GetComponent(typeof(Collider2D)) != Physics2D.OverlapPoint(inputPos)) return;

            var position = transform.position;
            _deltaX = inputPos.x - position.x;
            _deltaY = inputPos.y - position.y;
            _moveAllowed = true;
            _rb.freezeRotation = true;
            var sr = (SpriteRenderer) _rb.gameObject.GetComponent(typeof(SpriteRenderer));
            sr.sortingOrder = 50;
            /*_rb.velocity = new Vector2(0, 0);
            GetComponent<CircleCollider2D>().sharedMaterial = null;*/
        }

        private void OnDragging(Vector2 inputPos)
        {
            if (_moveAllowed)
                _rb.MovePosition(new Vector2(inputPos.x - _deltaX, inputPos.y - _deltaY));
        }

        public void OnDragEnd()
        {
            _moveAllowed = false;
            _rb.freezeRotation = false;
        }
    }
}