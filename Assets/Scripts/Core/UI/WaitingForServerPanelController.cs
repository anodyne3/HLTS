using UnityEngine;

namespace Core.UI
{
    public class WaitingForServerPanelController : PanelController
    {
        [SerializeField] private float time;
        [SerializeField] private float repeatRate;
        
        public override void Start()
        {
            backgroundButton.onClick.RemoveAllListeners();
        }

        public override void OpenPanel(params object[] args)
        {
            gameObject.SetActive(true);
            
            InvokeRepeating(nameof(StartTextAnimations), time, repeatRate);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
            
            CancelInvoke(nameof(StartTextAnimations));
        }
    }
}