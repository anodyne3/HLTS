using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class PanelController : GlobalAccess
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton;

        public virtual void OpenPanel()
        {
            AudioManager.PlayClip(SoundEffectType.OpenPanel);
            gameObject.SetActive(true);

            if (GameManager != null)
                GameManager.interactionEnabled = false;

            if (closeButton == null) return;

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
            
            if (backgroundButton == null) return;

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(ClosePanel);
        }

        protected virtual void ClosePanel()
        {
            AudioManager.PlayClip(SoundEffectType.ClosePanel);

            if (GameManager != null)
                GameManager.interactionEnabled = true;
            
            gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            if (PanelManager != null && PanelManager.AllPanels.Contains(this))
                PanelManager.AllPanels.Remove(this);
        }
    }
}