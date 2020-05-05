using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI.Prefabs
{
    public class ResourceRequirement : GlobalAccess
    {
        [SerializeField] private SVGImage icon;
        [SerializeField] private TMP_Text amountText;
        private Resource _required;

        public void Refresh(Resource resource)
        {
            _required = resource;
            icon.sprite = ResourceManager.GetCurrencySprite(resource.resourceType);
            var currentResourceAmount = PlayerData.GetResourceAmount(_required.resourceType); 
            amountText.text = currentResourceAmount >= _required.resourceAmount
                ? Constants.SufficientCurrencyPrefix
                : Constants.InsufficientCurrencyPrefix;
            amountText.text += currentResourceAmount + "</color> / " + _required.resourceAmount;
        }
    }
}