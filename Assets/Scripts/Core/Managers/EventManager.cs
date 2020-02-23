using System.Collections.Generic;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Managers
{
    public class EventManager : GlobalAccess
    {
        [HideInInspector] public GameEvent armPull;
        [HideInInspector] public GameEvent coinLoad;
        [HideInInspector] public GameEvent wheelResult;
        [HideInInspector] public GameEvent wheelRoll;
        
        [SerializeField] public List<GameEventListener> gameEventListeners = new List<GameEventListener>();
        
        private void OnEnable()
        {
            armPull = Resources.Load<GameEvent>("Events/armPullEvent");
            coinLoad = Resources.Load<GameEvent>("Events/coinLoadEvent");
            wheelResult = Resources.Load<GameEvent>("Events/wheelResultEvent");
            wheelRoll = Resources.Load<GameEvent>("Events/wheelRollEvent");
        }
        
        public void NewEventSubscription(GameObject parentObject, string gameEventName, UnityAction unityAction)
        {
            var newGameEvent = Resources.Load<GameEvent>("Events/" + gameEventName);
            var gameEventListener = parentObject.AddComponent<GameEventListener>();
            gameEventListener.@event = newGameEvent;
            gameEventListener.@event.RegisterListener(gameEventListener);
            gameEventListener.response.AddListener(unityAction);
            gameEventListeners.Add(gameEventListener);
        }
    }
}