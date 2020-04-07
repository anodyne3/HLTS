using System;
using Core.Input;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Core
{
    public class Gdpr : GlobalAccess//, IPointerClickHandler
        {
        [SerializeField] private Button confirmButton;
        [SerializeField] public TMP_Text textComponent;

        private void Start()
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmClicked);
            
            InputManager.Pressed += OnPressed;
            
            CheckConsent();
        }

        private void OnDisable()
        {
            InputManager.Pressed -= OnPressed;
        }

        private void CheckConsent()
        {
            var consentGiven = PlayerPrefs.HasKey(Constants.ConsentKey) &&
                               PlayerPrefs.GetInt(Constants.ConsentKey) == 1;

            if (consentGiven)
                ClosePanel();
        }

        private void ConfirmClicked()
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);

            PlayerPrefs.SetInt(Constants.ConsentKey, 1);
            PlayerPrefs.Save();

            ClosePanel();
        }

        private void OnPressed(PointerInput pointerInput)
        // public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.PlayClip(SoundEffectType.UiClick);

            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(textComponent, pointerInput.position, CameraManager.MainCamera);

            if (linkIndex == -1) return;

            var linkInfo = textComponent.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID() == "TermsOfService") Application.OpenURL(Urls.TermsOfService);
            else if (linkInfo.GetLinkID() == "PrivacyPolicy") Application.OpenURL(Urls.PrivacyPolicy);
        }

        private void ClosePanel()
        {
            gameObject.SetActive(false);

            FirebaseFunctionality.CheckLogin();
        }
    }
}