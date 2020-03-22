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
        [SerializeField] private TMP_Text coinsAmountText;

        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);

            RefreshCoins();
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshCoins);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, OpenPayoutPanel);
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenPayoutPanel()
        {
            PanelManager.OpenPanelSolo<PayoutPanelController>();
        }

        private static void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private void RefreshCoins()
        {
            coinsAmountText.text = PlayerData.coinsAmount.ToString();
        }
    }
}