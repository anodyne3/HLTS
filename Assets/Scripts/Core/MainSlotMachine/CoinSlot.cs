using System.Collections;
using Core.Input;
using Core.UI;
using Enums;
using UnityEngine;
using Utils;

namespace Core.MainSlotMachine
{
    public class CoinSlot : GlobalAccess
    {
        [SerializeField] private WorldSpaceButton upgradeButton;
        [SerializeField] private Transform insertedCoinStartPosition;
        [SerializeField] private Transform insertedCoinFinishPosition;
        public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        private CoinDragHandler _insertedCoin;
        private bool _loadingCoin;
        private float _coinLoadSpeed;
        
        private void Start()
        {
            upgradeButton.OnClick.RemoveAllListeners();
            upgradeButton.OnClick.AddListener(CoinSlotClicked);
        }

        private void CoinSlotClicked()
        {
            if (UpgradeManager.IsUpgradeMaxed(UpgradeTypes.CoinSlot)) return;
            
            PanelManager.OpenPanelSolo<UpgradePanelController>(UpgradeTypes.CoinSlot);
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (_loadingCoin || SlotMachine.CoinSlotFull || SlotMachine.autoMode) return;

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
            _insertedCoin.OnReleased();
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
                _insertedCoin.transform.position =
                    Vector2.Lerp(coinStart, insertedCoinStartPosition.position, curve.Evaluate(t));
                yield return null;
            }

            _insertedCoin.SetCoinOrderInLayer(Constants.CoinInsertedSortingOrder);
            _insertedCoin.SetCoinGravity(1.0f);

            while (_insertedCoin.transform.position.y > insertedCoinFinishPosition.position.y && t <= 5.0f)
            {
                t += Time.deltaTime * Constants.CoinLoadSpeed;

                yield return null;
            }

            _insertedCoin.gameObject.SetActive(false);
            ObjectPoolManager.coinPool.Release(_insertedCoin);
            EventManager.coinLoad.Raise();
            SlotMachine.coinsWereLoaded++;
            _loadingCoin = false;
        }
    }
}