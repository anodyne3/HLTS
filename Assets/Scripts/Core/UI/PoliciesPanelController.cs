using Core.Managers;
using Enums;
using UnityEngine;
using TMPro;
using Utils;

namespace Core.UI
{
    public class PoliciesPanelController : PanelController
    {
        [SerializeField] private TMP_Text textComponent;

        public override void Start()
        {
            base.Start();

            InputManager.Pressed += OnPressed;
        }
        
        private void OnDisable()
        {
            if (!InputManager.Instance) return;

            InputManager.Pressed -= OnPressed;
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
    }
}