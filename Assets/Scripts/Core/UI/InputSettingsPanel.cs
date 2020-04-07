using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class InputSettingsPanel : PanelController
    {
        [SerializeField] private CameraSettings cameraSettings;

        [SerializeField] private TMP_InputField dragRateTop;
        [SerializeField] private TMP_InputField dpiCompensate;
        [SerializeField] private TMP_InputField pivotLerpSpeed;
        [SerializeField] private TMP_InputField zoomLerpSpeed;
        [SerializeField] private TMP_InputField zoomMultiplier;
        [SerializeField] private TMP_InputField panMultiplier;
        [SerializeField] private TMP_InputField cazRate;
        [SerializeField] private TMP_InputField panRange;
        [SerializeField] private TMP_InputField pinchRate;
        [SerializeField] private TMP_InputField pinchIgnore;

        [SerializeField] private TMP_InputField cameraOffsetX;
        [SerializeField] private TMP_InputField cameraOffsetY;
        [SerializeField] private TMP_InputField cameraOffsetZ;
        [SerializeField] private TMP_InputField cameraAboveX;
        [SerializeField] private TMP_InputField cameraAboveY;
        [SerializeField] private TMP_InputField cameraAboveZ;
        [SerializeField] private TMP_InputField zoomMax;
        [SerializeField] private TMP_InputField zoomMin;
        [SerializeField] private TMP_InputField pivoterBaseX;
        [SerializeField] private TMP_InputField pivoterBaseY;
        [SerializeField] private TMP_InputField pivoterBaseZ;

        [SerializeField] private Button updateSettingsButton;

        public override void Awake()
        {
            base.Awake();

            /*dragRateTop.text = cameraSettings.dragRateTop.ToString();
            dpiCompensate.text = cameraSettings.dpiCompensate.ToString();
            pivotLerpSpeed.text = cameraSettings.pivotLerpSpeed.ToString();
            zoomLerpSpeed.text = cameraSettings.zoomLerpSpeed.ToString();
            zoomMultiplier.text = cameraSettings.zoomMultiplier.ToString();
            panMultiplier.text = cameraSettings.panMultiplier.ToString();
            cazRate.text = cameraSettings.cazRate.ToString();
            panRange.text = cameraSettings.panRange.ToString();
            pinchRate.text = cameraSettings.pinchRate.ToString();
            pinchIgnore.text = cameraSettings.pinchIgnore.ToString();
            cameraOffsetX.text = cameraSettings.cameraOffset.x.ToString();
            cameraOffsetY.text = cameraSettings.cameraOffset.y.ToString();
            cameraOffsetZ.text = cameraSettings.cameraOffset.z.ToString();
            cameraAboveX.text = cameraSettings.cameraAbove.x.ToString();
            cameraAboveY.text = cameraSettings.cameraAbove.y.ToString();
            cameraAboveZ.text = cameraSettings.cameraAbove.z.ToString();
            zoomMax.text = cameraSettings.zoomMax.ToString();
            zoomMin.text = cameraSettings.zoomMin.ToString();
            pivoterBaseX.text = cameraSettings.pivoterBase.x.ToString();
            pivoterBaseY.text = cameraSettings.pivoterBase.y.ToString();
            pivoterBaseZ.text = cameraSettings.pivoterBase.z.ToString();*/
        }

        public override void Start()
        {
            base.Start();
            
            updateSettingsButton.onClick.RemoveAllListeners();
            updateSettingsButton.onClick.AddListener(UpdateSettings);
        }

        private void UpdateSettings()
        {
            /*cameraSettings.dragRateTop = float.Parse(dragRateTop.text);
            cameraSettings.dpiCompensate = float.Parse(dpiCompensate.text);
            cameraSettings.pivotLerpSpeed = float.Parse(pivotLerpSpeed.text);
            cameraSettings.zoomLerpSpeed = float.Parse(zoomLerpSpeed.text);
            cameraSettings.zoomMultiplier = float.Parse(zoomMultiplier.text);
            cameraSettings.panMultiplier = float.Parse(panMultiplier.text);
            cameraSettings.cazRate = float.Parse(cazRate.text);
            cameraSettings.panRange = float.Parse(panRange.text);
            cameraSettings.pinchRate = float.Parse(pinchRate.text);
            cameraSettings.pinchIgnore = float.Parse(pinchIgnore.text);
            cameraSettings.zoomMax = float.Parse(zoomMax.text);
            cameraSettings.zoomMin = float.Parse(zoomMin.text);
            cameraSettings.cameraOffset.x = float.Parse(cameraOffsetX.text);
            cameraSettings.cameraOffset.y = float.Parse(cameraOffsetY.text);
            cameraSettings.cameraOffset.z = float.Parse(cameraOffsetZ.text);
            cameraSettings.cameraAbove.x = float.Parse(cameraAboveX.text);
            cameraSettings.cameraAbove.y = float.Parse(cameraAboveY.text);
            cameraSettings.cameraAbove.z = float.Parse(cameraAboveZ.text);
            cameraSettings.pivoterBase.x = float.Parse(pivoterBaseX.text);
            cameraSettings.pivoterBase.y = float.Parse(pivoterBaseY.text);
            cameraSettings.pivoterBase.z = float.Parse(pivoterBaseZ.text);*/
        }
            
    }
}