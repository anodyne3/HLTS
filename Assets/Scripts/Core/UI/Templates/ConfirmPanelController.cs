using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ConfirmPanelController : PanelController
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TweenPunchSetting punchSetting;
    
        public override void Start()
        {
            base.Start();
        
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmAction);
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(ClosePanel);
        }

        private void ConfirmAction()
        {
            punchSetting.DoPunch(confirmButton.transform, false);
            
            // confirm action function here
            
            //REMINDER!! - add tween punch setting and vertex animation scriptableObjects to prefab
            
            base.ClosePanel();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();
            
            punchSetting.DoPunch(cancelButton.transform, false);
        }
    }
}