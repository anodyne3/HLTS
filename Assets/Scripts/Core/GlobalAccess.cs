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
        protected static PlayerData PlayerData => PlayerData.Instance;
        protected static SlotMachine SlotMachine => SlotMachine.Instance;
        protected static CoinTray CoinTray => CoinTray.Instance;
    }
}