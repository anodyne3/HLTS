using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class OptionsPanelController : PanelController
    {
        [SerializeField] private ToggleButton soundsToggleButton;
        [SerializeField] private ToggleButton musicToggleButton;

        [SerializeField] private Button inputSettings;

        public override void Start()
        {
            base.Start();

            soundsToggleButton.ClearAndAddListener(ToggleSounds);
            musicToggleButton.ClearAndAddListener(ToggleMusic);

            if (GameManager != null && GameManager.debug)
            {
                inputSettings.gameObject.SetActive(true);
                inputSettings.onClick.RemoveAllListeners();
                inputSettings.onClick.AddListener(OpenInputSettings);
            }

            soundsToggleButton.RefreshToggle(AudioManager.soundsDisabled, true);
            musicToggleButton.RefreshToggle(AudioManager.musicDisabled, true);
        }

        private void ToggleSounds()
        {
            AudioManager.soundsDisabled = !AudioManager.soundsDisabled;
            soundsToggleButton.RefreshToggle(AudioManager.soundsDisabled);
        }

        private void ToggleMusic()
        {
            AudioManager.musicDisabled = !AudioManager.musicDisabled;
            musicToggleButton.RefreshToggle(AudioManager.musicDisabled);
        }

        private static void OpenInputSettings()
        {
            PanelManager.OpenSubPanel<InputSettingsPanel>();
        }
    }
}