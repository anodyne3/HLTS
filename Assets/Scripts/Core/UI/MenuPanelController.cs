using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class MenuPanelController : PanelController
    {
        [SerializeField] private Button accountButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button shirtStoreButton;
        [SerializeField] private Button creditsButton;

        public override void Start()
        {
            base.Start();

            accountButton.onClick.RemoveAllListeners();
            accountButton.onClick.AddListener(OpenAccountPanel);
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OpenOptionsPanel);
            shirtStoreButton.onClick.RemoveAllListeners();
            shirtStoreButton.onClick.AddListener(OpenShirtStore);
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(OpenLinkedIn);
        }

        private static void OpenAccountPanel()
        {
            PanelManager.OpenSubPanel<AccountPanelController>();
        }

        private static void OpenOptionsPanel()
        {
            PanelManager.OpenSubPanel<OptionsPanelController>();
        }
        
        private static void OpenShirtStore()
        {
            Application.OpenURL(Constants.ShirtBuyUrl);
        }
        
        private static void OpenLinkedIn()
        {
            Application.OpenURL(Constants.LinkedInUrl);
        }
    }
}