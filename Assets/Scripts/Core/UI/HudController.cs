using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class HudController : GlobalAccess
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button grabCoinButton;
        [SerializeField] private Button spendCoinButton;
        [SerializeField] private Button pullArmButton;
        [SerializeField] private TMP_Text coinsAmountText;

        private void Start()
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OpenMenuPanel);
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShopPanel);
            grabCoinButton.onClick.RemoveAllListeners();
            grabCoinButton.onClick.AddListener(GrabCoin);
            spendCoinButton.onClick.RemoveAllListeners();
            spendCoinButton.onClick.AddListener(SpendCoin);
            pullArmButton.onClick.RemoveAllListeners();
            pullArmButton.onClick.AddListener(PullArm);

            RefreshCoins();
        }

        private static void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private static void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private static void GrabCoin()
        {
            CoinTray.GrabCoin();
        }

        private static void SpendCoin()
        {
            SlotMachine.LoadCoin();
        }

        private static void PullArm()
        {
            SlotMachine.PullArm();
        }

        public void RefreshCoins()
        {
            var playerData2 = GlobalComponents.Instance.PlayerData2();
            coinsAmountText.text = playerData2.coinsAmount.ToString();
        }
    }
}