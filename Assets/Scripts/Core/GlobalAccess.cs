using Core.Managers;
using Core.UI;
using UnityEngine;
using Core.GameData;
using Core.MainSlotMachine;

namespace Core
{
    public class GlobalAccess : MonoBehaviour
    {
        protected static GameManager GameManager => Foundation.GameManager;
        protected static HudController HudController => Foundation.GetGlobalClass<HudController>();
        protected static PanelManager PanelManager => Foundation.GetGlobalClass<PanelManager>();
        protected static CameraManager CameraManager => Foundation.GetGlobalClass<CameraManager>();
        // protected static CameraManager CameraManager => CameraManager.Instance;
        protected static AudioManager AudioManager => GlobalComponents.Instance.AddGlobalComponent<AudioManager>() as AudioManager;
        // protected static AudioManager AudioManager => AudioManager.Instance;
        protected static ObjectPoolManager ObjectPoolManager => GlobalComponents.Instance.AddGlobalComponent<ObjectPoolManager>() as ObjectPoolManager;
        protected static FirebaseFunctionality FirebaseFunctionality => GlobalComponents.Instance.AddGlobalComponent<FirebaseFunctionality>() as FirebaseFunctionality;
        protected static SceneManager SceneManager => GlobalComponents.Instance.AddGlobalComponent<SceneManager>() as SceneManager;
        protected static ResourceManager ResourceManager => GlobalComponents.Instance.AddGlobalComponent<ResourceManager>() as ResourceManager;
        protected static EventManager EventManager => GlobalComponents.Instance.AddGlobalComponent<EventManager>() as EventManager;
        protected static PlayerData PlayerData => GlobalComponents.Instance.AddGlobalComponent<PlayerData>() as PlayerData;
        protected static SlotMachine SlotMachine => GlobalComponents.Instance.AddGlobalComponent<SlotMachine>() as SlotMachine;
        protected static CoinTray CoinTray => GlobalComponents.Instance.AddGlobalComponent<CoinTray>() as CoinTray;
    }
}