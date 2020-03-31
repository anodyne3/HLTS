using System;
using DG.Tweening;
using MyScriptableObjects;
using TMPro;
using UnityEngine;

namespace Core.UI
{
        [Serializable]
        public struct ToggleButton
        {
            [SerializeField] private Transform transform;
            [SerializeField] private SVGImage image;
            [SerializeField] private TMP_Text text;
            [SerializeField] private TweenPunchSetting punchSetting;
            
            public void RefreshToggle(bool value, bool start = false)
            {
                image.color = value ? Color.red : Color.green;
                text.text = value ? "off" : "on";
                
                if (start) return;
                
                transform.DOPunchScale(punchSetting.punchAmount, punchSetting.punchDuration);
            }
    }
}