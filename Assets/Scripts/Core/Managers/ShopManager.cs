using Core.UI;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Utils;

namespace Core.Managers
{
    public class ShopManager : GlobalClass, IStoreListener
    {
        [HideInInspector] public bool canValidateReceipt = true;    // true for prod
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        [HideInInspector] public ShopPanelController shopPanel;
        [HideInInspector] public ShopProduct[] shopProducts;

        public void Start()
        {
            shopPanel = PanelManager.GetPanel<ShopPanelController>();
            
            LoadShopProducts();
            NarrativeManager.GetNarrativeTestData();
            
            if (_storeController != null) return;
            
            InitialisePurchasing();
        }

        private void LoadShopProducts()
        {
            shopProducts = GeneralUtils.SortLoadedList<ShopProduct>(Constants.ShopProductPath,
                (x, y) => x.ResourceCost.CompareTo(y.ResourceCost));
        }
        
        private void InitialisePurchasing()
        {
            if (IsInitialised()) return;

            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            builder.AddProducts(shopPanel.LoadProductDefinitions());
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialised()
        {
            return _storeController != null && _extensionProvider != null;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"Initialization Failure Reason: {error}");
        }

        public ProductMetadata GetLocalizedCurrencyPrice(string productId)
        {
            return _storeController.products.WithID(productId).metadata;
        }

        public void PurchaseProduct(string productId)
        {
            _storeController.InitiatePurchase(productId);
        }

        public void CompletePurchase(string productId)
        {
            if (productId == null) return;

            var product = _storeController.products.WithID(productId);
            
            if (product != null)
                _storeController.ConfirmPendingPurchase(product);

            
            AlertMessage.Init(Constants.PurchaseMessage);

            EventManager.refreshCurrency.Raise();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var validPurchase = true;

            if (canValidateReceipt)
            {
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                    AppleTangle.Data(), Application.identifier);

                try
                {
                    var result = validator.Validate(e.purchasedProduct.receipt);
                    // For informational purposes, we list the receipt(s) - kill this log once this all works
                    Debug.Log("Receipt is valid. Contents:");
                    foreach (var productReceipt in result)
                    {
                        Debug.Log(productReceipt.productID);
                        Debug.Log(productReceipt.purchaseDate);
                        Debug.Log(productReceipt.transactionID);
                    }
                }
                catch (IAPSecurityException)
                {
                    //message to user
                    Debug.Log("Invalid receipt, not unlocking content");
                    validPurchase = false;
                }
            }

            if (validPurchase)
            {
                FirebaseFunctionality.PurchaseProduct(e.purchasedProduct.definition.id);
            }

            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            //message to user
            Debug.Log($"Purchase of {i} Failed: {p}.");
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;

            //kill this log once it all works 
            Debug.Log($"Store Initialised: {controller}; {extensions}");
        }
    }
}