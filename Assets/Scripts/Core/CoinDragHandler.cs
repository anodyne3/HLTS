using UnityEngine;

namespace Core
{
    public class CoinDragHandler : GlobalAccess
    {
        private float _deltaX, _deltaY, _startingGravity;
        private bool _moveAllowed;
        private SpriteRenderer _spriteRenderer;

        [HideInInspector] public Rigidbody2D RigidBody2D { get; private set; }
        [HideInInspector] public CircleCollider2D CircleCollider { get; private set; }

        private void Awake()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            RigidBody2D = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
            CircleCollider = (CircleCollider2D) GetComponent(typeof(CircleCollider2D));
            
            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;

            _startingGravity = RigidBody2D.gravityScale;
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
            if (CircleCollider != Physics2D.OverlapPoint(pointerPosition)) return;

            CameraManager.draggingDisabled = true;

            // var position = transform.position;
            // _deltaX = pointerPosition.x - position.x;
            // _deltaY = pointerPosition.y - position.y;
            _moveAllowed = true;
            RigidBody2D.freezeRotation = true;
            _spriteRenderer.sortingOrder = 50;
            CircleCollider.isTrigger = true;
            SetCoinGravity(0.0f);
        }

        private void OnDragged(Vector2 pointerPosition)
        {
            if (_moveAllowed)
                RigidBody2D.MovePosition(new Vector2(pointerPosition.x, pointerPosition.y));
        }

        public void OnReleased()
        {
            CircleCollider.isTrigger = false;
            _moveAllowed = false;
            RigidBody2D.freezeRotation = false;
            SetCoinGravity(_startingGravity);
        }

        public void SetCoinGravity(float value)
        {
            RigidBody2D.gravityScale = value;
        }

        public void SetCoinOrderInLayer(int value)
        {
            _spriteRenderer.sortingOrder = value;
        }
    }
}