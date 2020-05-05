using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Input
{
    [Serializable]
    public class WorldSpaceButton : GlobalAccess
    {
        public Collider2D buttonCollider2D;  
        public UnityEvent onClick;
        public bool isClicked;

        private void Start()
        {
            buttonCollider2D = (Collider2D) GetComponent(typeof(Collider2D));
            
            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
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
            if (buttonCollider2D != Physics2D.OverlapPoint(pointerPosition)) return;

            if (GameManager != null)
            {
                if (!GameManager.interactionEnabled) return;
                
                GameManager.interactionEnabled = false;
            }

            isClicked = true;
        }
        
        //to const and inputSettings
        public float dragIgnore = 0.1f;
        
        private void OnDragged(Vector2 pointerPosition)
        {
            if(!isClicked) return;
            
            if (GameManager != null && !GameManager.interactionEnabled)
            {
                OnReleased();
                return;
            }

            /*
            if (InputManager.dragDelta.sqrMagnitude > dragIgnore)
            {
                isClicked = false;
                OnReleased();
            }
            */
        }
        
        private void OnReleased()
        {
            Release();

            if (GameManager == null) return;

            GameManager.interactionEnabled = true;
        }

        private void Release()
        {
            // if (onClick.GetPersistentEventCount() == 0) return;

            if (isClicked && buttonCollider2D == Physics2D.OverlapPoint(InputManager.pointerPosition))
                onClick.Invoke();

            isClicked = false;
        }
    }
}