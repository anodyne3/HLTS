using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.UI
{
    public class PanelManager : GlobalClass
    {
        public readonly List<PanelController> allPanels = new List<PanelController>();

        public override void Awake()
        {
            base.Awake();

            foreach (Transform panel in transform)
                if (panel.TryGetComponent(out PanelController panelController))
                    allPanels.Add(panelController);
        }
        
        public void OpenPanelSolo<T>(params object[] args) where T : PanelController
        {
            foreach (var panel in allPanels)
            {
                if (panel.TryGetComponent(out T requiredPanel))
                    requiredPanel.OpenPanel();
                else
                    panel.gameObject.SetActive(false);
            }
        }

        public T GetPanel<T>() where T : PanelController
        {
            return (from Transform t in transform select t.GetComponent<T>()).FirstOrDefault(currentPopup =>
                currentPopup != null);
        }
    }
}