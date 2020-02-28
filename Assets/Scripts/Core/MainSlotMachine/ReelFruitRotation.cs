using System;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class ReelFruitRotation : GlobalAccess
    {
        private SpriteRenderer _spriteRenderer;
        private const float SortingOrderThreshold = 50.0f;
        private Vector3 _eulerAngles;

        private void Start()
        {
            _spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
        }

        private void Update()
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