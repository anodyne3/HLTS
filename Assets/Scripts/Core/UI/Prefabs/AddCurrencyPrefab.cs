using Unity.VectorGraphics;
using UnityEngine;

namespace Core.UI.Prefabs
{
    public class AddCurrencyPrefab : GlobalAccess
    {
        public SVGImage currencyIcon;

        public void Init(Sprite sprite)
        {
            currencyIcon.sprite = sprite;
        }
    }
}