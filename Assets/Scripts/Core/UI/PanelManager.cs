using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.GameData;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;

namespace Core.UI
{
    public class PanelManager : GlobalClass
    {
        [SerializeField] private WaitingForServerPanelController waitingForServerPanel;
        [SerializeField]private TweenPunchSetting generalButtonPunchSetting;
        
        public readonly List<PanelController> allPanels = new List<PanelController>();
        public TweenSetting openPanelTweenSettings;
        public TweenSetting closePanelTweenSettings;
        public TweenFadeSetting openPanelFadeSettings;
        public TweenFadeSetting closePanelFadeSettings;

        [HideInInspector] public bool dragPinchDisabled;

        public override void Awake()
        {
            base.Awake();

            foreach (Transform panel in transform)
                if (panel.TryGetComponent(out PanelController panelController))
                    allPanels.Add(panelController);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, OpenPayoutPanel);

            if (!PlayerData.NarrativeIsComplete()) return;
            
            var narrativePanel = GetPanel<NarrativePanelController>();

            if (narrativePanel == null) return;
            allPanels.Remove(narrativePanel);
            Destroy(narrativePanel.gameObject);
        }
        
        public void WaitingForServerPanel(bool show = true)
        {
            if (show)
                waitingForServerPanel.OpenPanel();
            else
                waitingForServerPanel.HidePanel();
        }

        private readonly WaitUntil _gameManagerInteractionWait = new WaitUntil(() => GameManager.interactionEnabled);
        private void OpenPayoutPanel()
        {
            OpenPanelOnHold<PayoutPanelController>(_gameManagerInteractionWait);
        }

        public void OpenPanelOnHold<T>(WaitUntil waitUntil, params object[] args) where T : PanelController
        {
            StartCoroutine(OpenOnHold<T>(waitUntil, args));
        }

        private IEnumerator OpenOnHold<T>(CustomYieldInstruction waitUntil, params object[] args) where T : PanelController
        {
            var keepWaiting = waitUntil.keepWaiting;
            if (GameManager != null && keepWaiting)
                yield return waitUntil;

            OpenPanelSolo<T>(args);
        } 

        public void OpenPanelSolo<T>(params object[] args) where T : PanelController
        {
            foreach (var panel in allPanels)
            {
                if (panel.TryGetComponent(out T requiredPanel))
                    requiredPanel.OpenPanel(args);
                else
                {
                    panel.isOpen = false;
                    panel.gameObject.SetActive(false);
                }
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
                if (panel.isOpen)
                    openPanelCount++;
            }

            return openPanelCount;
        }

        public void PunchButton(Transform target)
        {
            generalButtonPunchSetting.DoPunch(target, false);
        }
    }
}