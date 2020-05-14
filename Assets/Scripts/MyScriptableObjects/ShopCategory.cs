using System;
using UnityEngine;

namespace MyScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "Category", menuName = "MyAssets/ShopCategory", order = 71)]
    public class ShopCategory : ScriptableObject
    {
        public int orderInShop;
        public static int order;

        private void OnEnable()
        {
            order = orderInShop;
        }

        public int GetOrder()
        {
            return order;
        }
    }
}