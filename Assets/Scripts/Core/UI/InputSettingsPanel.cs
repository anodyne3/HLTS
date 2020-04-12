using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class InputSettingsPanel : PanelController
    {
        [SerializeField] private CameraSettings cameraSettings;

        [SerializeField] private TMP_InputField panMultiplier;
        [SerializeField] private TMP_InputField panLerpSpeed;
        [SerializeField] private TMP_InputField zoomLerpSpeed;
        [SerializeField] private TMP_InputField zoomMultiplier;
        [SerializeField] private TMP_InputField panRange;
        [SerializeField] private TMP_InputField pinchRate;
        [SerializeField] private TMP_InputField pinchIgnore;

        [SerializeField] private Button updateSettingsButton;
        [SerializeField] private TweenPunchSetting punchSetting;

        public override void Awake()
        {
            base.Awake();

            panMultiplier.text = cameraSettings.panMultiplier.ToString();
            panLerpSpeed.text = cameraSettings.panLerpSpeed.ToString();
            zoomLerpSpeed.text = cameraSettings.zoomLerpSpeed.ToString();
            zoomMultiplier.text = cameraSettings.zoomMultiplier.ToString();
            panRange.text = cameraSettings.panRange.ToString();
            pinchRate.text = cameraSettings.pinchRate.ToString();
            pinchIgnore.text = cameraSettings.pinchIgnore.ToString();

            var settingsParent = panelTransform.GetChild(1).transform;
            foreach (Transform setting in settingsParent)
            {
                var header = (TMP_Text) setting.GetChild(1).GetComponent(typeof(TMP_Text));
                header.text = setting.name;
            }
        }

        public override void Start()
        {
            base.Start();
            
            updateSettingsButton.onClick.RemoveAllListeners();
            updateSettingsButton.onClick.AddListener(UpdateSettings);
        }

        private void UpdateSettings()
        {
            cameraSettings.panMultiplier = float.Parse(panMultiplier.text);
            cameraSettings.panLerpSpeed = float.Parse(panLerpSpeed.text);
            cameraSettings.zoomLerpSpeed = float.Parse(zoomLerpSpeed.text);
            cameraSettings.zoomMultiplier = float.Parse(zoomMultiplier.text);
            cameraSettings.panRange = float.Parse(panRange.text);
            cameraSettings.pinchRate = float.Parse(pinchRate.text);
            cameraSettings.pinchIgnore = float.Parse(pinchIgnore.text);
            punchSetting.DoPunch(updateSettingsButton.transform);
        }
    }
}