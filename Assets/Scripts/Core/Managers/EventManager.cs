using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Managers
{
    public class EventManager : GlobalAccess
    {
        public GameEvent armPull;
        public GameEvent coinLoad;
        public GameEvent wheelResult;
        public GameEvent wheelRoll;
        
        private void OnEnable()
        {
            armPull = Resources.Load<GameEvent>("Events/armPullEvent");
            coinLoad = Resources.Load<GameEvent>("Events/coinLoadEvent");
            wheelResult = Resources.Load<GameEvent>("Events/wheelResultEvent");
            wheelRoll = Resources.Load<GameEvent>("Events/wheelRollEvent");
        }
        
        public void NewEventSubscription(string gameEventName, UnityAction action)
        {
            var armPullEvent = Resources.Load<GameEvent>("Events/" + gameEventName);
            GameEventListener.NewGameEventListener(gameObject, armPullEvent, action);
        }
    }
}