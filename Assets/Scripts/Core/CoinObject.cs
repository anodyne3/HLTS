using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class CoinObject : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        //public Collider coinCollider;
        private PointerEventData _pointerEventData;

        public void OnDrag(PointerEventData eventData)
        {
            _pointerEventData = eventData;
        }

        public void LoadCoinIntoSlot()
        {
            OnEndDrag(_pointerEventData);
            gameObject.SetActive(false);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }
    }
}