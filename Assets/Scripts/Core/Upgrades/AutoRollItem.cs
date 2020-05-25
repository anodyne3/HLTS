using System;
using Core.Input;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Core.Upgrades
{
    [Serializable]
    public class AutoRollItem : MonoBehaviour
    {
        public WorldSpaceButton button;
        [SerializeField] private SpriteRenderer buttonSprite;
        [SerializeField] private SpriteRenderer brokenSprite;
        public Light2D light2d;
        private UpgradeIndicator _upgradeIndicator;

        private void Start()
        {
            button = (WorldSpaceButton) GetComponent(typeof(WorldSpaceButton));
            light2d = (Light2D) GetComponent(typeof(Light2D));
            _upgradeIndicator = (UpgradeIndicator) GetComponentInChildren(typeof(UpgradeIndicator)); 
            light2d.enabled = false;
            buttonSprite.color = Color.gray;
        }

        public void ButtonLit(bool value)
        {
            if (brokenSprite.enabled) return;
            
            buttonSprite.color = value ? Color.white : Color.gray;
            light2d.enabled = value;
        }

        public void RepairButton()
        {
            brokenSprite.enabled = false;
            _upgradeIndicator.gameObject.SetActive(false);
        }
    }
}