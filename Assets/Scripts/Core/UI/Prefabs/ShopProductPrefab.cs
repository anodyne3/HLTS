using Enums;
using MyScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI.Prefabs
{
    public class ShopProductPrefab : GlobalAccess
    {
        [SerializeField] private Button purchaseProductButton;
        [SerializeField] private Transform buttonIcon;
        [SerializeField] private TMP_Text productName;
        [SerializeField] private TMP_Text productAmount;
        [SerializeField] private SVGImage productIcon;
        [SerializeField] private TMP_Text resourceCost;

        private ShopProduct _shopProduct;

        private void Start()
        {
            purchaseProductButton.onClick.RemoveAllListeners();
            purchaseProductButton.onClick.AddListener(PurchaseProduct);
        }

        public void Init(ShopProduct shopProduct)
        {
            _shopProduct = shopProduct;

            RefreshProduct();
        }

        private void RefreshProduct()
        {
            productIcon.sprite = _shopProduct.productIcon;
            productName.text = _shopProduct.productName;

            if (_shopProduct.ResourceType != ResourceType.HardCurrency)
            {
                resourceCost.text = Constants.GetCurrencySpriteAsset(_shopProduct.ResourceType);
                resourceCost.text += _shopProduct.ResourceCost;
            }
            else
            {
                productAmount.text = _shopProduct.ResourceCost.ToString();
                var productMetaData = ShopManager.GetLocalizedCurrencyPrice(_shopProduct.productId);
                resourceCost.text = productMetaData.localizedPriceString;
            }
        }

        private void PurchaseProduct()
        {
            if (_shopProduct.ResourceType != ResourceType.HardCurrency)
                if (!HasResourcesForPurchase())
                    AlertMessage.Init(Constants.LowResourcesPrefix +
                                      Constants.GetResourceTypeName(_shopProduct.ResourceType));
                else
                    PanelManager.OpenSubPanel<ConfirmPurchasePanelController>(_shopProduct);

            else
            {
                ShopManager.PurchaseProduct(_shopProduct.productId);
                PanelManager.PunchButton(buttonIcon);
            }
        }

        private bool HasResourcesForPurchase()
        {
            return CurrencyManager.GetCurrencyAmount(_shopProduct.ResourceType) >= _shopProduct.ResourceCost;
        }
    }
}