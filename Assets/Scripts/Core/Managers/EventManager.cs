using System.Collections.Generic;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Core.Managers
{
    public class EventManager : GlobalAccess
    {
        [HideInInspector] public GameEvent armPull;
        [HideInInspector] public GameEvent coinConsume;
        [HideInInspector] public GameEvent coinLoad;
        [HideInInspector] public GameEvent wheelResult;
        [HideInInspector] public GameEvent wheelRoll;
        
        [SerializeField] public List<GameEventListener> gameEventListeners = new List<GameEventListener>();
        
        private void OnEnable()
        {
            armPull = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.armPullEvent);
            coinConsume = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinConsumeEvent);
            coinLoad = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinLoadEvent);
            wheelResult = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.wheelResultEvent);
            wheelRoll = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.wheelRollEvent);
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