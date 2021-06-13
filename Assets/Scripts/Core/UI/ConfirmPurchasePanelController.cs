using MyScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ConfirmPurchasePanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;
        [SerializeField] private TMP_Text productName;
        [SerializeField] private SVGImage productIcon;
        [SerializeField] private TMP_Text resourceCost;

        private ShopProduct _shopProduct;
    
        public override void Start()
        {
            base.Start();
        
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmAction);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            _shopProduct = (ShopProduct) args[0];
            
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            productName.text = _shopProduct.productName;
            productIcon.sprite = _shopProduct.productIcon;
            resourceCost.text = Constants.GetCurrencySpriteAsset(_shopProduct.ResourceType) + _shopProduct.ResourceCost;
        }

        private void ConfirmAction()
        {
            punchSetting.DoPunch(confirmButton.transform, false);
            
            FirebaseFunctionality.PurchaseProduct(_shopProduct.productId).ConfigureAwait(false);

            base.ClosePanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}