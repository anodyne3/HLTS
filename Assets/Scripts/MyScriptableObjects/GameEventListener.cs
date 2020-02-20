using UnityEngine;
using UnityEngine.Events;

namespace MyScriptableObjects
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent @event;
        public UnityEvent response;

        public GameEventListener()
        {
            response = new UnityEvent();
        }

        public static void NewGameEventListener(GameObject parentObject, GameEvent gameEvent, UnityAction unityAction)
        {
            var gameEventListener = parentObject.AddComponent<GameEventListener>();
            gameEventListener.@event = gameEvent;
            gameEventListener.@event.RegisterListener(gameEventListener);
            gameEventListener.response.AddListener(unityAction);
        }

        private void OnEnable()
        {
            if (!@event) return;
            
            @event.RegisterListener(this);
        }

        private void OnDestroy()
        {
            if (!@event) return;

            @event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            response.Invoke();
        }
    }
}