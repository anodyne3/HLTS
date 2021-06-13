using System;
using Core.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Input
{
    [Serializable]
    public class WorldSpaceButton : GlobalAccess
    {
        public Collider2D buttonCollider2D;
        public class WorldSpaceButtonClickedEvent : UnityEvent {}
        public WorldSpaceButtonClickedEvent OnClick
        {
            get => onClick;
            set => onClick = value;
        }
        
        [SerializeField]
        private WorldSpaceButtonClickedEvent onClick = new WorldSpaceButtonClickedEvent();

        protected WorldSpaceButton()
        {}
        
        public bool isClicked;

        private void Start()
        {
            buttonCollider2D = (Collider2D) GetComponent(typeof(Collider2D));

            InputManager.Pressed += OnPressed;
            InputManager.Released += OnReleased;
        }

        private void OnDestroy()
        {
            if (!InputManager.Instance) return;

            InputManager.Pressed -= OnPressed;
            InputManager.Released -= OnReleased;
        }

        private void OnPressed(Vector2 pointerPosition)
        {
            if (buttonCollider2D != Physics2D.OverlapPoint(pointerPosition)) return;

            if (GameManager != null && !GameManager.interactionEnabled) return;

            isClicked = true;
        }

        private void OnReleased()
        {
            Release();
        }

        private void Release()
        {
            if (GameManager != null && !GameManager.interactionEnabled)
                isClicked = false;
            
            if (isClicked && buttonCollider2D == Physics2D.OverlapPoint(InputManager.pointerPosition))
                onClick.Invoke();

            isClicked = false;
        }
    }
}