using Core.Managers;
using Core.UI;
using UnityEngine;
using Core.GameData;

namespace Core
{
    public class GlobalAccess : MonoBehaviour
    {
        protected static GameManager GameManager => Foundation.GameManager;
        protected static PanelManager PanelManager => Foundation.GetGlobalClass<PanelManager>();
        protected static CameraManager CameraManager => CameraManager.Instance;
        protected static AudioManager AudioManager => AudioManager.Instance;
        protected static PlayerData PlayerData => GlobalComponents.Instance.AddGlobalComponent<PlayerData>() as PlayerData;

        protected static PlayerData PlayerDataDefo;
        //protected static PlayerData PlayerData => PlayerData.Instance;
        //protected static SlotMachine SlotMachine => SlotMachine.Instance;
        protected static SlotMachine SlotMachine => GlobalComponents.Instance.AddGlobalComponent<SlotMachine>() as SlotMachine;
        protected static CoinTray CoinTray => CoinTray.Instance;
        
        protected static EventManager EventManager => GlobalComponents.Instance.AddGlobalComponent<EventManager>() as EventManager;

        private void OnEnable()
        {
            PlayerDataDefo = GlobalComponents.Instance.PlayerData2();
        }
    }
}