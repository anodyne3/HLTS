using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Prefabs
{
    public class AddCurrencyPrefab : GlobalAccess
    {
        public Image currencyIcon;

        public void Init(Sprite sprite)
        {
            currencyIcon.sprite = sprite;
        }
    }
}