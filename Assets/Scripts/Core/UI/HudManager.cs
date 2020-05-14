using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class HudManager : GlobalClass
    {
        [SerializeField] private Button menuButton;
        
        [SerializeField] private Button chestButton;
        
        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
           
            chestButton.onClick.RemoveAllListeners();
            chestButton.onClick.AddListener(OpenChestPanel);
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenChestPanel()
        {
            PanelManager.OpenPanelSolo<ChestInventoryPanelController>();
        }
    }
}