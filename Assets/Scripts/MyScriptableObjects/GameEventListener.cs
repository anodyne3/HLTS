using Core;
using UnityEngine.Events;

namespace MyScriptableObjects
{
    public class GameEventListener : GlobalAccess
    {
        public GameEvent @event;
        public UnityEvent response;

        public GameEventListener()
        {
            response = new UnityEvent();
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