using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class HudController : GlobalAccess
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button pullArmButton;
        [SerializeField] private TMP_Text coinsAmountText;

        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);
            pullArmButton.onClick.RemoveAllListeners();
            pullArmButton.onClick.AddListener(PullArm);

            RefreshCoins();
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.armPullEvent, RefreshCoins);
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private static void PullArm()
        {
            SlotMachine.PullArm();
        }

        public void RefreshCoins()
        {
            coinsAmountText.text = PlayerData.coinsAmount.ToString();
        }
    }
}