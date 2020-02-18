using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Core
{
    public class Gdpr : GlobalAccess, IPointerClickHandler
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] public TMP_Text textComponent;

        private void Start()
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmClicked);
            
            CheckConsent();
        }

        private void CheckConsent()
        {
            var consentGiven = PlayerPrefs.HasKey(Constants.ConsentKey) && PlayerPrefs.GetInt(Constants.ConsentKey) == 1;
            
            if (consentGiven)
                ClosePanel();
        }

        private void ConfirmClicked()
        {
            PlayerPrefs.SetInt(Constants.ConsentKey, 1);
            PlayerPrefs.Save();

            ClosePanel();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, CameraManager.MainCamera);
            
            if (linkIndex == -1) return;

            var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            
            if (linkInfo.GetLinkID() == "TermsOfService") Application.OpenURL(Urls.TermsOfService);
            else if (linkInfo.GetLinkID() == "PrivacyPolicy") Application.OpenURL(Urls.PrivacyPolicy);
        }

        private void ClosePanel()
        {
            gameObject.SetActive(false);
            
            FirebaseCheckAndFixDependencies.Login();
        }
    }
}