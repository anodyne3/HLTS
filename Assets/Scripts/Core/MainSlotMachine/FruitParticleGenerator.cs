using System;
using System.Collections;
using Boo.Lang;
using Core.UI;
using UnityEngine;
using Utils;
using Enums;
using Random = UnityEngine.Random;

namespace Core.MainSlotMachine
{
    public class FruitParticleGenerator : GlobalAccess
    {
        [SerializeField] private FruitParticle fruitPrefab;
        [SerializeField] private FruitBurst[] fruitBursts;
        [SerializeField] private Vector2 startingVelocity;
        [SerializeField] private float lifeSpan;

        private List<Coroutine> _particleEmitters = new List<Coroutine>();

        private void Start()
        {
            ObjectPoolManager.fruitBurstPool =
                new MyObjectPool<FruitParticle>(() => Instantiate(fruitPrefab, transform));

            InitBursts();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutStartEvent, StartEmitter);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutFinishEvent, StopEmitter);
        }

        private void InitBursts()
        {
            fruitBursts = new[]
            {
                new FruitBurst(0.0f, 7, 15, 0, 1.1f),
                new FruitBurst(1.0f, 7, 15, 0, 2.5f),
                new FruitBurst(2.0f, 7, 15, 0, 3.0f)
            };
        }

        private void StartEmitter()
        {
            RefreshParticles();

            var fruitBurstsLength = fruitBursts.Length;
            for (var i = 0; i < fruitBurstsLength; i++)
            {
                var newParticleEmitter = StartCoroutine(nameof(EmitParticles), fruitBursts[i]);
                _particleEmitters.Add(newParticleEmitter);
            }
        }

        private void RefreshParticles()
        {
            if (SlotMachine.payout == FruitType.None) return;

            fruitPrefab = ResourceManager.GetFruitSprite(SlotMachine.payout);
        }

        private void StopEmitter()
        {
            StopAllCoroutines();
        }

        private IEnumerator EmitParticles(FruitBurst fruitBurst)
        {
            var initialWait = new WaitForSeconds(fruitBurst.burstTime);
            var intervalWait = new WaitForSeconds(fruitBurst.burstInterval);
            var currentCycle = 0;
            var transformPosition = transform.position;
            while (fruitBurst.cycles >= currentCycle)
            {
                yield return initialWait;

                var particleAmount = Random.Range(fruitBurst.burstAmountMin,
                    fruitBurst.burstAmountMax + 1);
                for (var i = 0; i < particleAmount; i++)
                {
                    var newParticle = ObjectPoolManager.fruitBurstPool.Get();
                    newParticle.lifeSpan = lifeSpan;
                    newParticle.transform.position = transformPosition;
                    newParticle.rigidBody2D.velocity = startingVelocity;

                    //newParticle.spriteRenderer.sprite = ResourceManager.GetFruitSprite(SlotMachine.payout).spriteRenderer.sprite;
                    newParticle.spriteRenderer.sortingOrder += ObjectPoolManager.fruitBurstPool.PoolCount();
                }
                
                if (fruitBurst.cycles > 0)
                    currentCycle++;
                
                yield return intervalWait;
            }
        }
    }
}