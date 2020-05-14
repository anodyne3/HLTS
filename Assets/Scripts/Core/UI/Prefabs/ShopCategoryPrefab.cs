using TMPro;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class ShopCategoryPrefab : GlobalAccess
    {
        [SerializeField] private TMP_Text categoryName;
        [SerializeField] private TMP_Text categoryNameUnderlay;
        public Transform productHolder;

        public void Init(string nameText)
        {
            categoryName.text = nameText;
            categoryNameUnderlay.text = nameText;
        }
    }
}