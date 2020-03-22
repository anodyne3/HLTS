using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class PanelController : GlobalAccess
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton;
        public TMP_TextAnimation[] _textAnimations;

        public virtual void Awake()
        {
            var _textAnimationsLength = _textAnimations.Length;
            for (var i = 0; i < _textAnimationsLength; i++)
            {
                _textAnimations[i].Init();
            }
        }
        
        public virtual void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(ClosePanel);
            }

            if (backgroundButton == null) return;

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(ClosePanel);
        }

        public virtual void OpenPanel()
        {
            AudioManager.PlayClip(SoundEffectType.OpenPanel);
            gameObject.SetActive(true);

            if (GameManager != null)
                GameManager.interactionEnabled = false;
        }

        protected void StartTextAnimations()
        {
            var textAnimationsLength = _textAnimations.Length;
            for (var i = 0; i < textAnimationsLength; i++)
            {
                _textAnimations[i].vertexAnimation.RefreshTargetText();
                StartCoroutine(_textAnimations[i].vertexAnimation.AnimateTargetText());
            }
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
            if (PanelManager != null && PanelManager.allPanels.Contains(this))
                PanelManager.allPanels.Remove(this);
        }
    }
}