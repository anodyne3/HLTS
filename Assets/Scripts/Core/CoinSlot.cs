using System.Collections;
using UnityEngine;
using Utils;

namespace Core
{
    public class CoinSlot : GlobalAccess
    {
        [SerializeField] private CoinDragHandler _insertedCoin;
        [SerializeField] private bool _loadingCoin;
        
        private void LoadCoin()
        {
            StartCoroutine(MoveCoinIntoSlot());
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.collider.TryGetComponent(typeof(CoinDragHandler), out var component)) return;

            _insertedCoin = (CoinDragHandler) component;
            
            TakeControlOfCoin();
            LoadCoin();
            EventManager.coinLoad.Raise();
        }

        private void TakeControlOfCoin()
        {
            if (_loadingCoin) return;
            
            _insertedCoin.OnDragEnd();
        }

        private IEnumerator MoveCoinIntoSlot()
        {
            _insertedCoin.transform.position = Vector2.Lerp(_insertedCoin.transform.position, Constants.CoinSlotStartPosition, Constants.CoinLoadSpeed);

            yield return null;
        }
    }
}