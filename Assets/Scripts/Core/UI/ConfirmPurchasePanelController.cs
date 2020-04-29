using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ConfirmPurchasePanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;
        [SerializeField] private SVGImage productIcon;
        [SerializeField] private TMP_Text currencyCost;

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
            productIcon.sprite = _shopProduct.productIcon;
            currencyCost.text = _shopProduct.productCost.ToString();
        }

        private void ConfirmAction()
        {
            punchSetting.DoPunch(confirmButton.transform, false);
            
            // confirm action function here
            //FirebaseFunctionality.shopTransaction(_shopProduct)

            base.ClosePanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}