using UnityEngine;
using UnityEngine.Purchasing;
using Enums;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "ShopProduct", menuName = "MyAssets/ShopProduct", order = 70)]
    public class ShopProduct : ScriptableObject
    {
        public string productName;
        public string productId;
        public ProductType productType;
        public int productCost;
        public ResourceType currencyType;
        public Sprite productIcon;
    }
}