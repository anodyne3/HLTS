using Core.GameData;
using Core.Managers;
using Core.UI;
using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core
{
    public class Gdpr : PanelController
    {
        [SerializeField] private TMP_Text textComponent;

        public override void Start()
        {
            base.Start();

            backgroundButton.onClick.RemoveAllListeners();

            InputManager.Pressed += OnPressed;

            CheckConsent();
            // StartTextAnimations();
        }

        private void OnDisable()
        {
            if (!InputManager.Instance) return;

            InputManager.Pressed -= OnPressed;
        }

        private void CheckConsent()
        {
            if (!PlayerData.ConsentGiven) return;
            
            gameObject.SetActive(false);
            FirebaseFunctionality.Init();
        }

        private void OnPressed(Vector2 pointerPosition)
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);

            pointerPosition = CameraManager.MainCamera.WorldToScreenPoint(pointerPosition);

            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(textComponent, pointerPosition, CameraManager.MainCamera);

            if (linkIndex == -1) return;

            var linkInfo = textComponent.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID() == "TermsOfService") Application.OpenURL(Constants.TermsAndConditionsUrl);
            else if (linkInfo.GetLinkID() == "PrivacyPolicy") Application.OpenURL(Constants.PrivacyPolicyUrl);
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            PlayerPrefs.SetInt(Constants.ConsentKey, 1);
            PlayerPrefs.Save();

            FirebaseFunctionality.Init();
        }
    }
}