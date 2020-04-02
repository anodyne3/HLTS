using Core.Managers;
using UnityEngine;

namespace Core
{
    public class CoinDragHandler : InputManager
    {
        private float _deltaX, _deltaY;
        private bool _moveAllowed;
        private SpriteRenderer _spriteRenderer;

        [HideInInspector] public Vector2 mousePos;
        public Rigidbody2D RigidBody2D { get; private set; }
        public CircleCollider2D CircleCollider { get; private set; }

        private void Awake()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            RigidBody2D = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
            CircleCollider = (CircleCollider2D) GetComponent(typeof(CircleCollider2D));
        }

        private void FixedUpdate()
        {
            Dragging();
        }

        protected override void OnDragBegin(Vector2 inputPos)
        {
            base.OnDragBegin(inputPos);
            
            if (CircleCollider != Physics2D.OverlapPoint(inputPos)) return;

            CameraManager.draggingDisabled = true;

            var position = transform.position;
            _deltaX = inputPos.x - position.x;
            _deltaY = inputPos.y - position.y;
            _moveAllowed = true;
            RigidBody2D.freezeRotation = true;
            _spriteRenderer.sortingOrder = 50;
            CircleCollider.isTrigger = true;
        }

        protected override void OnDragging(Vector2 inputPos)
        {
            base.OnDragging(inputPos);
            
            if (_moveAllowed)
                RigidBody2D.MovePosition(new Vector2(inputPos.x - _deltaX, inputPos.y - _deltaY));
        }

        public override void OnDragEnd()
        {
            base.OnDragEnd();
            
            CircleCollider.isTrigger = false;
            _moveAllowed = false;
            RigidBody2D.freezeRotation = false;
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