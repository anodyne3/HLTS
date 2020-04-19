using UnityEngine;
using UnityEngine.Purchasing;

namespace Core.UI
{
    public class ShopPanelController : PanelController, IStoreListener
    {
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        
        private ProductDefinition[] _productDefinitions;
        
        public override void Start()
        {
            base.Start();

            LoadProductDefinitions();

            if (_storeController != null) return;

            InitialisePurchasing();
        }

        private void LoadProductDefinitions()
        {
            var shopProductsLength = ResourceManager.shopProducts.Length;
            
            _productDefinitions = new ProductDefinition[shopProductsLength];

            for (var i = 0; i < shopProductsLength; i++)
            {
                _productDefinitions[i] = new ProductDefinition(ResourceManager.shopProducts[i].productId,
                    ResourceManager.shopProducts[i].productType);
            }
        }
        
        private void InitialisePurchasing()
        {
            if (IsInitialised()) return;

            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            builder.AddProducts(_productDefinitions);
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialised()
        {
            return _storeController != null && _extensionProvider != null;
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
            Debug.Log($"Store Initialised: {controller}; {extensions}");
        }
    }
}