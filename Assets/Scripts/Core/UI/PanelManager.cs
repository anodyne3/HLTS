using System.Collections.Generic;
using System.Linq;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class PanelManager : GlobalClass
    {
        public readonly List<PanelController> allPanels = new List<PanelController>();
        public TweenSetting openPanelTweenSettings;
        public TweenSetting closePanelTweenSettings;
        public TweenFadeSetting openPanelFadeSettings;
        public TweenFadeSetting closePanelFadeSettings;

        public override void Awake()
        {
            base.Awake();

            foreach (Transform panel in transform)
                if (panel.TryGetComponent(out PanelController panelController))
                    allPanels.Add(panelController);
            
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, OpenPayoutPanel);
        }

        private void OpenPayoutPanel()
        {
            OpenPanelSolo<PayoutPanelController>();
        }

        public void OpenPanelSolo<T>(params object[] args) where T : PanelController
        {
            foreach (var panel in allPanels)
            {
                if (panel.TryGetComponent(out T requiredPanel))
                    requiredPanel.OpenPanel(args);
                else
                    panel.gameObject.SetActive(false);
            }
        }

        public T GetPanel<T>() where T : PanelController
        {
            return (from Transform t in transform select t.GetComponent<T>()).FirstOrDefault(currentPopup =>
                currentPopup != null);
        }

        public void OpenSubPanel<T>(params object[] args) where T : PanelController
        {
            var requiredPanel = GetPanel<T>();
            requiredPanel.transform.SetAsLastSibling();
            requiredPanel.OpenPanel(args);
        }

        public int OpenPanelCount()
        {
            var openPanelCount = 0;

            var allPanelsCount = allPanels.Count;
            for (var i = 0; i < allPanelsCount; i++)
            {
                var panel = allPanels[i];
                if (panel.isActiveAndEnabled)
                    openPanelCount++;
            }

            return openPanelCount;
        } 
    }
}