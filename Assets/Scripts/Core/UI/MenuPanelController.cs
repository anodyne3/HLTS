using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MenuPanelController : PanelController
    {
        [SerializeField] private Button accountButton;
        [SerializeField] private Button optionsButton;

        public override void Start()
        {
            base.Start();

            accountButton.onClick.RemoveAllListeners();
            accountButton.onClick.AddListener(OpenAccountPanel);
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(OpenOptionsPanel);
        }

        private static void OpenAccountPanel()
        {
            PanelManager.OpenSubPanel<AccountPanelController>();
        }

        private static void OpenOptionsPanel()
        {
            PanelManager.OpenSubPanel<OptionsPanelController>();
        }
    }
}