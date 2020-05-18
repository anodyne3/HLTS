using Core.UI.Prefabs;
using Enums;
using MyScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils;

namespace Core.UI
{
    public class ShopPanelController : PanelController
    {
        [Header("Prefabs")] [SerializeField] private Transform categoryHolder;
        [SerializeField] private ShopCategoryPrefab categoryPrefab;
        [SerializeField] private ShopProductPrefab hardCurrencyProductPrefab;
        [SerializeField] private ShopProductPrefab softCurrencyProductPrefab;

        private ShopCategory[] _shopCategories;
        private ProductDefinition[] _hardCurrencyProductDefinitions;

        public override void Start()
        {
            base.Start();

            InitCategories();
        }

        public override void OpenPanel(params object[] args)
        {
            if (isActiveAndEnabled)
            {
                ClosePanel();
                return;
            }

            base.OpenPanel(args);
        }

        private void InitCategories()
        {
            _shopCategories = GeneralUtils.SortLoadedList<ShopCategory>(Constants.ShopCategoryPath,
                (x, y) => x.orderInShop.CompareTo(y.orderInShop));

            var shopCategoriesLength = _shopCategories.Length;
            for (var i = 0; i < shopCategoriesLength; i++)
            {
                var newCategory = Instantiate(categoryPrefab, categoryHolder);
                newCategory.Init(_shopCategories[i].name);
                newCategory.name = _shopCategories[i].name;
            }

            InitProducts();
        }

        private void InitProducts()
        {
            var shopProductsLength = ShopManager.shopProducts.Length;
            for (var i = 0; i < shopProductsLength; i++)
            {
                var newShopPanelItem =
                    Instantiate(
                        ShopManager.shopProducts[i].ResourceType == ResourceType.HardCurrency
                            ? hardCurrencyProductPrefab
                            : softCurrencyProductPrefab, SelectCategoryTransform(i));
                newShopPanelItem.Init(ShopManager.shopProducts[i]);
            }
        }

        public IEnumerable<ProductDefinition> LoadProductDefinitions()
        {
            var hardProductList = new List<ShopProduct>();

            var shopProductsLength = ShopManager.shopProducts.Length;
            for (var i = 0; i < shopProductsLength; i++)
            {
                if (ShopManager.shopProducts[i].ResourceType == ResourceType.HardCurrency)
                    hardProductList.Add(ShopManager.shopProducts[i]);
            }

            _hardCurrencyProductDefinitions = new ProductDefinition[hardProductList.Count];

            var hardProductListCount = hardProductList.Count;
            for (var i = 0; i < hardProductListCount; i++)
            {
                _hardCurrencyProductDefinitions[i] = new ProductDefinition(hardProductList[i].productId,
                    hardProductList[i].productType);
            }

            return _hardCurrencyProductDefinitions;
        }

        private Transform SelectCategoryTransform(int index)
        {
            var childIndex = ShopManager.shopProducts[index].shopCategory.orderInShop;
            var shopCategoryPrefab = (ShopCategoryPrefab) categoryHolder
                .GetChild(childIndex).GetComponent(typeof(ShopCategoryPrefab));
            return shopCategoryPrefab.productHolder;
        }
    }
}