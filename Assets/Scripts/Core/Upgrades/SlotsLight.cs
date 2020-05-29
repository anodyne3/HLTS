using Core.Input;
using Core.UI;
using Enums;
using UnityEngine;
using Utils;

namespace Core.Upgrades
{
    public class SlotsLight : GlobalAccess
    {
        [SerializeField] private WorldSpaceButton upgradeButton;
            
        private void Start()
        {
            upgradeButton.OnClick.RemoveAllListeners();
            upgradeButton.OnClick.AddListener(CoinSlotClicked);
        }

        private static void CoinSlotClicked()
        {
            if (UpgradeManager.IsUpgradeMaxed(UpgradeTypes.SlotsLight))
                Application.OpenURL(Constants.ShirtClaimUrl);
            else
                PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.SlotsLight);
        }
    }
}