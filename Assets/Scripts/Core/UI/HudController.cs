using MyScriptableObjects;
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
        [SerializeField] private TMP_Text CoinsAmountText;

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

            CoinsAmountText.text = PlayerData.CoinsAmount.ToString();
        }

        private void OpenMenuPanel()
        {
            PanelManager.OpenPanelSolo<MenuPanelController>();
        }

        private void OpenShopPanel()
        {
            PanelManager.OpenPanelSolo<ShopPanelController>();
        }

        private void GrabCoin()
        {
            GameManager.GrabCoin();
        }

        private void SpendCoin()
        {
            GameManager.SpendCoin();
        }

        private void PullArm()
        {
            GameManager.PullArm();
        }
    }
}