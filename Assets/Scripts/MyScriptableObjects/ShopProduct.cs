using UnityEngine;
using UnityEngine.Purchasing;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "ShopProduct", menuName = "MyAssets/ShopProduct", order = 70)]
    public class ShopProduct : ScriptableObject
    {
        public string productName;
        public string productId;
        public ProductType productType;
    }
}