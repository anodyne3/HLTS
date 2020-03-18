using System.Collections.Generic;
using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Event", menuName = "MyAssets/GameEvent", order = 10)]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public void Raise()
        {
            var listenersCount = _listeners.Count;
            for (var i = listenersCount - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }
}