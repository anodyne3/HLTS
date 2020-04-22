using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ChestPanelController : PanelController
    {
        [SerializeField] private Button claimChestButton;
    
        public override void Start()
        {
            base.Start();

            claimChestButton.onClick.RemoveAllListeners();
            claimChestButton.onClick.AddListener(ClaimChest);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            claimChestButton.interactable = PlayerData.currentChestRoll >= Constants.LoChestRoll;
        }

        private static void ClaimChest()
        {
            if (PlayerData.currentChestRoll < Constants.HiChestRoll)
            {
                PanelManager.OpenSubPanel<ConfirmClaimChestPanelController>();
            }
            else
                FirebaseFunctionality.ClaimChest();
        }
    }
}