using Core;
using MyScriptableObjects;
using UnityEngine;

namespace Utils
{
    public class EventManagerButtonMaker : GlobalAccess
    {
        [SerializeField] private Transform eventButtonHolder;
        [SerializeField] private GameEventPrefab eventButtonPrefab;
        
        [ContextMenu("MakeButtons")]
        public void MakeButtons()
        {
            var oldButtons = eventButtonHolder.GetComponentsInChildren(typeof(GameEventPrefab));
            
            foreach (var child in oldButtons)
            {
                DestroyImmediate(child.gameObject);
            }
            
            var gameEvents = Resources.LoadAll<GameEvent>("Events");

            foreach (var gameEvent in gameEvents)
            {
                var newEventPrefab = Instantiate(eventButtonPrefab, eventButtonHolder);
                newEventPrefab.buttonText.text = System.Text.RegularExpressions.Regex.Replace(gameEvent.name, "([A-Z])",
                    " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                newEventPrefab.gameEvent = gameEvent;
                newEventPrefab.gameObject.name = gameEvent.name;
            }
        }
    }
}