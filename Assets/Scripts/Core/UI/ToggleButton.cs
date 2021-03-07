using System;
using MyScriptableObjects;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    [Serializable]
    public class ToggleButton : MonoBehaviour
    {
        public Button toggleButton;
        [SerializeField] private SVGImage image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private TweenPunchSetting punchSetting;

        public void ClearAndAddListener(UnityAction action)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(action);
        }

        public void RefreshToggle(bool value, bool start = false)
        {
            image.color = value ? Constants.toggleOff : Constants.toggleOn;
            text.text = value ? "off" : "on";

            if (start) return;

            punchSetting.DoPunch(toggleButton.transform, false);
        }
    }
}