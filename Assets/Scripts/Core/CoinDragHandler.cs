using System;
using System.Diagnostics;
using UnityEngine;

namespace Core
{
    public class CoinDragHandler : GlobalAccess
    {
        private float _deltaX, _deltaY;
        private Rigidbody2D _rigidBody2D;
        private bool _moveAllowed;
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _coinCollider;

        public Vector2 mousePos;
        public Rigidbody2D RigidBody2D => _rigidBody2D;
        public CircleCollider2D CircleCollider => _coinCollider;

        private void Start()
        {
            _rigidBody2D = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
            _coinCollider = (CircleCollider2D) GetComponent(typeof(CircleCollider2D));
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
            if (_coinCollider != Physics2D.OverlapPoint(inputPos)) return;

            var position = transform.position;
            _deltaX = inputPos.x - position.x;
            _deltaY = inputPos.y - position.y;
            _moveAllowed = true;
            _rigidBody2D.freezeRotation = true;
            _spriteRenderer = (SpriteRenderer) _rigidBody2D.gameObject.GetComponent(typeof(SpriteRenderer));
            _spriteRenderer.sortingOrder = 50;
            /*_rb.velocity = new Vector2(0, 0);*/
            _coinCollider.isTrigger = true;
        }

        private void OnDragging(Vector2 inputPos)
        {
            if (_moveAllowed)
                _rigidBody2D.MovePosition(new Vector2(inputPos.x - _deltaX, inputPos.y - _deltaY));
        }

        public void OnDragEnd()
        {
            _coinCollider.isTrigger = false;
            _moveAllowed = false;
            _rigidBody2D.freezeRotation = false;
        }

        public void SetCoinGravity(float value)
        {
            _rigidBody2D.gravityScale = value;
        }
        
        public void SetCoinOrderInLayer(int value)
        {
            _spriteRenderer.sortingOrder = value;
        }
    }
}