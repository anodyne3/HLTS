using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class GeneralPanelController : PanelController
    {
        [SerializeField] private ToggleButton ToggleButton;
        
        [SerializeField] private Button Button;
        [SerializeField] private Button Toggle;
        
        public TMP_Text footer;

        private bool _replaceThisValue;

        public override void Start()
        {
            base.Start();

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(ButtonAction);
            Toggle.onClick.RemoveAllListeners();
            Toggle.onClick.AddListener(ToggleAction);
            
            ToggleButton.RefreshToggle(_replaceThisValue, true);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();
            
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            // refresh panel function here
            
            //footer.text = ;
        }

        private static void ButtonAction()
        {
            // button action function here
            
            //REMINDER!! - add tween punch setting and vertex animation scriptableObjects to prefab
        }

        private void ToggleAction()
        {
            _replaceThisValue = !_replaceThisValue;
            ToggleButton.RefreshToggle(_replaceThisValue);
        }
    }
}