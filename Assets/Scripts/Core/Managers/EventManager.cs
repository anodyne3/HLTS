using System.Collections.Generic;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Core.Managers
{
    public class EventManager : GlobalAccess
    {
        [HideInInspector] public GameEvent autoSlotMode;
        [HideInInspector] public GameEvent coinInsert;
        [HideInInspector] public GameEvent testEvent;
        
        [HideInInspector] public GameEvent armPull;
        [HideInInspector] public GameEvent coinConsume;
        [HideInInspector] public GameEvent coinCreated;
        [HideInInspector] public GameEvent coinDropped;
        [HideInInspector] public GameEvent coinLoad;
        [HideInInspector] public GameEvent generateCoin;
        [HideInInspector] public GameEvent payoutFinish;
        [HideInInspector] public GameEvent payoutStart;
        [HideInInspector] public GameEvent refreshUi;
        [HideInInspector] public GameEvent userEarnedReward;
        [HideInInspector] public GameEvent wheelResult;
        [HideInInspector] public GameEvent wheelRoll;

        [SerializeField] public List<GameEventListener> gameEventListeners = new List<GameEventListener>();
        
        private void OnEnable()
        {
            autoSlotMode = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.autoSlotModeEvent);
            coinInsert = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinInsertEvent);
            testEvent = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.testEvent);
            
            armPull = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.armPullEvent);
            coinConsume = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinConsumeEvent);
            coinCreated = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinCreatedEvent);
            coinDropped = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinDroppedEvent);
            coinLoad = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.coinLoadEvent);
            generateCoin = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.generateCoinEvent);
            payoutFinish = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.payoutFinishEvent);
            payoutStart = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.payoutStartEvent);
            refreshUi = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.refreshUiEvent);
            userEarnedReward = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.userEarnedRewardEvent);
            wheelResult = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.wheelResultEvent);
            wheelRoll = Resources.Load<GameEvent>("Events/" + Constants.GameEvents.wheelRollEvent);
        }
        
        public GameEventListener NewEventSubscription(GameObject parentObject, string gameEventName, UnityAction unityAction)
        {
            var newGameEvent = Resources.Load<GameEvent>("Events/" + gameEventName);
            var gameEventListener = parentObject.AddComponent<GameEventListener>();
            gameEventListener.@event = newGameEvent;
            gameEventListener.@event.RegisterListener(gameEventListener);
            gameEventListener.response.AddListener(unityAction);
            gameEventListeners.Add(gameEventListener);

            return gameEventListener;
        }
    }
}