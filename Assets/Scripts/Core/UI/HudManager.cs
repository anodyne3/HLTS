using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class HudManager : GlobalClass
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button chestButton;
        
        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);
            chestButton.onClick.RemoveAllListeners();
            chestButton.onClick.AddListener(OpenChestPanel);
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private static void OpenChestPanel()
        {
            PanelManager.OpenPanelSolo<ChestInventoryPanelController>();
        }
    }
}