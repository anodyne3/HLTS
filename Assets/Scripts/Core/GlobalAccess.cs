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
        protected static CurrencyManager CurrencyManager => Foundation.GetGlobalClass<CurrencyManager>();
        protected static PanelManager PanelManager => Foundation.GetGlobalClass<PanelManager>();
        protected static CameraManager CameraManager => Foundation.GetGlobalClass<CameraManager>();
        protected static ChestManager ChestManager => Foundation.GetGlobalClass<ChestManager>();
        protected static UpgradeRepairManager UpgradeRepairManager => Foundation.GetGlobalClass<UpgradeRepairManager>();
        protected static InputManager InputManager => GlobalComponents.Instance.AddGlobalComponent<InputManager>() as InputManager;
        protected static AdManager AdManager => GlobalComponents.Instance.AddGlobalComponent<AdManager>() as AdManager;
        protected static AudioManager AudioManager => GlobalComponents.Instance.AddGlobalComponent<AudioManager>() as AudioManager;
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