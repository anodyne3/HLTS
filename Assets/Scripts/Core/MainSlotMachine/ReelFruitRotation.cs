using System;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ReelFruitRotation : GlobalAccess
    {
        private SpriteRenderer _spriteRenderer;
        private const float SortingOrderThreshold = 54.0f;
        private Vector3 _eulerAngles;

        private void Start()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinCreatedEvent, InitialAdjustSortingLayer);
        }

        private void InitialAdjustSortingLayer()
        {
            AdjustSortingLayer();
            var eventListener = GetComponent(typeof(GameEventListener));
            Destroy(eventListener);
        }

        private void LateUpdate()
        {
            if (!SlotMachine.wheelsAreRolling) return;

            AdjustSortingLayer();
        }

        private void AdjustSortingLayer()
        {
            _eulerAngles = transform.rotation.eulerAngles;

            if (_eulerAngles.x < SortingOrderThreshold && Math.Abs(_eulerAngles.y) < Constants.WorldSpaceTolerance)
                _spriteRenderer.sortingOrder = 2;

            else if (_eulerAngles.x > 360.0f - SortingOrderThreshold &&
                     Mathf.Abs(_eulerAngles.y) < Constants.WorldSpaceTolerance)
                _spriteRenderer.sortingOrder = 2;

            else
                _spriteRenderer.sortingOrder = 0;
        }
    }
}