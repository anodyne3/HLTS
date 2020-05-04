using System;
using Core.Input;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Core.UpgradeRepairs
{
    [Serializable]
    public class AutoRollItem
    {
        public WorldSpaceButton button;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Light2D light;

        private void Start()
        {
            button.onClick.RemoveAllListeners();
        }

        public void EnableButton(bool value)
        {
            light.enabled = value;
            button.interactable = value;
        }
    }
}