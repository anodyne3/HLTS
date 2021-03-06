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
        [SerializeField] private UpgradeIndicator upgradeIndicator;
        public Light2D light2d;
        public bool hasLight;

        private void Start()
        {
            button = (WorldSpaceButton) GetComponent(typeof(WorldSpaceButton));
            buttonSprite.color = Color.gray;

            if (!hasLight) return;
            
            light2d = (Light2D) GetComponent(typeof(Light2D));
            light2d.enabled = false;
        }

        public void SpriteLit(bool value)
        {
            if (brokenSprite.enabled) return;
            
            buttonSprite.color = value ? Color.white : Color.gray;
        }
        
        public void LightLit(bool value)
        {
            if (brokenSprite.enabled || !hasLight) return;
            
            light2d.enabled = value;
        }

        public void RepairButton()
        {
            brokenSprite.enabled = false;
            upgradeIndicator.gameObject.SetActive(false);
        }
    }
}