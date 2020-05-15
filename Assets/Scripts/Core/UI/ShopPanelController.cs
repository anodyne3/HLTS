using System.Collections.Generic;
using Core.UI.Prefabs;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils;

namespace Core.UI
{
    public class ShopPanelController : PanelController, IStoreListener
    {
        [Header("Prefabs")] [SerializeField] private Transform categoryHolder;
        [SerializeField] private ShopCategoryPrefab categoryPrefab;
        [SerializeField] private ShopProductPrefab productPrefab;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        private ShopCategory[] _shopCategories;
        private ProductDefinition[] _hardCurrencyProductDefinitions;
        private ShopProduct[] _shopProducts;

        public override void Start()
        {
            base.Start();

            InitCategories();

            LoadProductDefinitions();

            if (_storeController != null) return;

            InitialisePurchasing();
        }

        public void LoadShopProducts()
        {
            _shopProducts = GeneralUtils.SortLoadedList<ShopProduct>(Constants.ShopProductPath,
                (x, y) => x.productCost.CompareTo(y.productCost));
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel(args);

            if (!isActiveAndEnabled) return;

            ClosePanel();
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
        }

        private void LoadProductDefinitions()
        {
            var hardProductList = new List<ShopProduct>();

            var shopProductsLength = _shopProducts.Length;
            for (var i = 0; i < shopProductsLength; i++)
            {
                var newShopPanelItem = Instantiate(productPrefab, SelectCategoryTransform(i));
                newShopPanelItem.Init(_shopProducts[i]);

                if (_shopProducts[i].currencyType == ResourceType.HardCurrency)
                    hardProductList.Add(_shopProducts[i]);
            }

            _hardCurrencyProductDefinitions = new ProductDefinition[hardProductList.Count];

            var hardProductListCount = hardProductList.Count;
            for (var i = 0; i < hardProductListCount; i++)
            {
                _hardCurrencyProductDefinitions[i] = new ProductDefinition(_shopProducts[i].productId,
                    _shopProducts[i].productType);
            }
        }

        private Transform SelectCategoryTransform(int index)
        {
            var childIndex = _shopProducts[index].shopCategory.orderInShop;
            var shopCategoryPrefab = (ShopCategoryPrefab) categoryHolder
                .GetChild(childIndex).GetComponent(typeof(ShopCategoryPrefab));
            return shopCategoryPrefab.productHolder;
        }

        private void InitialisePurchasing()
        {
            if (IsInitialised()) return;

            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            builder.AddProducts(_hardCurrencyProductDefinitions);
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialised()
        {
            return _storeController != null && _extensionProvider != null;
        }

        public static void CompletePurchase(object product)
        {
            if (product == null) return;

            EventManager.refreshCurrency.Raise();
        }

        public void OnQueryInventoryFailed(string message)
        {
            Debug.Log($"Query Inventory Failed: {message}");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"Initialization Failure Reason: {error}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log($"PurchaseProcessingResult: {e}");
            throw new System.Exception();
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log($"Purchase of {i} Failed: {p}.");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;

            Debug.Log($"Store Initialised: {controller}; {extensions}");
        }
    }
}