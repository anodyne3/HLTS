using UnityEngine;
using UnityEngine.Purchasing;
using Enums;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "ShopProduct", menuName = "MyAssets/ShopProduct", order = 70)]
    public class ShopProduct : ScriptableObject
    {
        public ResourceType currencyType;
        public ShopCategory shopCategory;
        public ProductType productType;
        public string productId;
        public string productName;
        public Sprite productIcon;
        public int productCost;
    }
}