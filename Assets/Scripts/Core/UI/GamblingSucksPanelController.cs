using Core.GameData;
using Core.Managers;
using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class GamblingSucksPanelController : PanelController
    {
        [SerializeField] private TMP_Text textComponent;
        
        public override void Start()
        {
            base.Start();
            
            backgroundButton.onClick.RemoveAllListeners();
            
            InputManager.Pressed += OnPressed;
            
            InitPanel();
        }
        
        private void OnDisable()
        {
            if (!InputManager.Instance) return;

            InputManager.Pressed -= OnPressed;
        }

        private void InitPanel()
        {
            if (!PlayerData.WarningRead) return;
            
            gameObject.SetActive(false);
            PanelManager.OpenPanelSolo<Gdpr>();
        }

        private void OnPressed(Vector2 pointerPosition)
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);

            pointerPosition = CameraManager.MainCamera.WorldToScreenPoint(pointerPosition);

            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(textComponent, pointerPosition, CameraManager.MainCamera);

            if (linkIndex == -1) return;

            var linkInfo = textComponent.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID() == "GamblingSupport") Application.OpenURL(Constants.GamblingSupport);
        }

        protected override void ClosePanel()
        {
            PlayerPrefs.SetInt(Constants.WarningKey, 1);
            PlayerPrefs.Save();
            
            base.ClosePanel();
            
            PanelManager.OpenPanelSolo<Gdpr>();
        }
    }
}