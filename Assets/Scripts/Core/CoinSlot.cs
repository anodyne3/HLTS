using System.Collections;
using UnityEngine;
using Utils;

namespace Core
{
    public class CoinSlot : GlobalAccess
    {
        private CoinDragHandler _insertedCoin;
        private bool _loadingCoin;
        [SerializeField] private Transform insertedCoinStartPosition;
        [SerializeField] private Transform insertedCoinFinishPosition;

        public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        private float _coinLoadSpeed;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_loadingCoin || SlotMachine.coinIsLoaded) return;
            
            if (!other.TryGetComponent(typeof(CoinDragHandler), out var component)) return;
            
            _loadingCoin = true;
            _insertedCoin = (CoinDragHandler) component;
            
            TakeControlOfCoin();
            LoadCoin();
        }

        private void TakeControlOfCoin()
        {
            _insertedCoin.SetCoinGravity(0.0f);
            _insertedCoin.RigidBody2D.velocity = Vector2.zero;
            _insertedCoin.OnDragEnd();
        }

        private void LoadCoin()
        {
            _coinLoadSpeed = Mathf.Clamp(
                (_insertedCoin.transform.position - insertedCoinStartPosition.position).sqrMagnitude,
                0.1f, 0.9f);
            
            StartCoroutine(nameof(MoveCoinIntoSlot));
        }

        private IEnumerator MoveCoinIntoSlot()
        {
            _insertedCoin.CircleCollider.enabled = false;
            var coinStart = _insertedCoin.transform.position;
            var t = 0.0f;
            var animationTimeLength = curve[curve.length - 1].time;
            while (t <= animationTimeLength)
            {
                t += Time.deltaTime / _coinLoadSpeed;
                _insertedCoin.transform.position = Vector2.Lerp(coinStart, insertedCoinStartPosition.position, curve.Evaluate(t));
                yield return null;
            }

            _insertedCoin.SetCoinOrderInLayer(11);
            _insertedCoin.SetCoinGravity(1.0f);

            while (_insertedCoin.transform.position.y > insertedCoinFinishPosition.position.y && t <= 5.0f)
            {
                t += Time.deltaTime * Constants.CoinLoadSpeed;
                
                yield return null;
            }
            
            _insertedCoin.gameObject.SetActive(false);
            ObjectPoolManager.coinPool.Release(_insertedCoin);
            EventManager.coinLoad.Raise();
            _loadingCoin = false;
        }
    }
}