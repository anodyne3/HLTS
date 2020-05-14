using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI.Prefabs
{
    public class ShopProductPrefab : GlobalAccess
    {
        [SerializeField] private Button purchaseProductButton;
        [SerializeField] private SVGImage productIcon;
        [SerializeField] private TMP_Text productName;
        [SerializeField] private TMP_Text productCost;

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

            if (_shopProduct.currencyType != ResourceType.HardCurrency)
                productCost.text = Constants.GetCurrencySpriteAsset(_shopProduct.currencyType);

            productCost.text += " " + _shopProduct.productCost;
        }

        private void PurchaseProduct()
        {
            if (_shopProduct.currencyType != ResourceType.HardCurrency)
                FirebaseFunctionality.PurchaseProduct(_shopProduct.productId);
        }
    }
}