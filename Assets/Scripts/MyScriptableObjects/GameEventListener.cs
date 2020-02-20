using UnityEngine;
using UnityEngine.Events;

namespace MyScriptableObjects
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent @event;
        public UnityEvent response;

        private void OnEnable()
        {
            @event.RegisterListener(this);
        }

        private void OnDestroy()
        {
            //if (Event == null) return;
            
            @event.UnregisterListener(this);
        }
        
        public void OnEventRaised()
        {
            response.Invoke();            
        }
    }
}