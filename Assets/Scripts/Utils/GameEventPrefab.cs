using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class GameEventPrefab : MonoBehaviour
    {
        public Button eventButton;
        public TMP_Text buttonText;
        public GameEvent gameEvent;

        private void OnEnable()
        {
            eventButton.onClick.RemoveAllListeners();
            eventButton.onClick.AddListener(gameEvent.Raise);
        }
    }
}