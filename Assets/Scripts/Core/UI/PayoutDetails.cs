using Core.Input;
using UnityEngine;

namespace Core.UI
{
    public class PayoutDetails : GlobalAccess
    {
        [SerializeField] private WorldSpaceButton openButton;
    
        private void Start()
        {
            openButton = (WorldSpaceButton) GetComponent(typeof(WorldSpaceButton));
             
            openButton.OnClick.RemoveAllListeners();
            openButton.OnClick.AddListener(PayoutDetailsClicked);
        }
    
        private static void PayoutDetailsClicked()
        {
            PanelManager.OpenPanelSolo<PayoutDetailsPanelController>();
        }
    }
}
