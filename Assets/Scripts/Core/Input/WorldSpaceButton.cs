using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Input
{
    [Serializable]
    public class WorldSpaceButton : GlobalAccess
    {
        private Collider2D _buttonCollider2D;
        public UnityEvent onClick;
        public bool interactable;

        private void Start()
        {
            _buttonCollider2D = (Collider2D) GetComponent(typeof(Collider2D));
            
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
            if (_buttonCollider2D != Physics2D.OverlapPoint(pointerPosition)) return;

            if (CameraManager.draggingDisabled) return;

            InputManager.startPosition = pointerPosition;
        }
        
        //to const
        // private float DragIgnore = 0.1f;
        
        private void OnDragged(Vector2 pointerPosition)
        {
            if (CameraManager.draggingDisabled) return;

            InputManager.dragDelta = InputManager.startPosition - pointerPosition;
            InputManager.startPosition = pointerPosition;
            
            //dead zone?
            //if (InputManager.dragDelta.sqrMagnitude > DragIgnore) return;
        }
        
        private void OnReleased()
        {
            Release();

            CameraManager.draggingDisabled = false;
            
            if (GameManager == null) return;

            GameManager.interactionEnabled = true;
        }

        private void Release()
        {
            if (onClick.GetPersistentEventCount() == 0 || !interactable) return;

            onClick.Invoke();
        }
    }
}