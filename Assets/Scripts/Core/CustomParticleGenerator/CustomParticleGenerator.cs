using System.Collections;
using Core.Managers;
using Core.UI;
using MyScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.CustomParticleGenerator
{
    public class CustomParticleGenerator : GlobalAccess
    {
        [SerializeField] private CustomParticle fruitPrefab;
        [SerializeField] private ParticleBurst[] fruitBursts;

        private MyObjectPool<CustomParticle> _particlePool;

        private void Start()
        {
            _particlePool = ObjectPoolManager.CreateObjectPool<CustomParticle>(fruitPrefab, transform);
        }

        public void StartEmitter()
        {
            var payoutTypeCount = SlotMachine.payoutType.Count;
            for (var i = 0; i < payoutTypeCount; i++)
            {
                var payoutSprite = ResourceManager.GetFruitParticleSprite(SlotMachine.payoutType[i]);
                StartCoroutine(EmitParticles(fruitBursts[i], payoutSprite));
            }
        }

        public void StopEmitter()
        {
            StopAllCoroutines();
        }

        private IEnumerator EmitParticles(ParticleBurst fruitBurst, Sprite payoutSprite)
        {
            //performance enhancing vars
            var initialWait = new WaitForSeconds(fruitBurst.burstTime);
            var intervalWait = new WaitForSeconds(fruitBurst.burstInterval);
            var randomStartingVelocity = new Vector2(); 
            var currentCycle = 0;
            var startingPosition = transform.position + fruitBurst.startPositionOffset;

            while (fruitBurst.cycles >= currentCycle)
            {
                yield return initialWait;

                var particleAmount = Random.Range(fruitBurst.burstAmountMin,
                    fruitBurst.burstAmountMax + 1);
                for (var i = 0; i < particleAmount; i++)
                {
                    var newParticle = (CustomParticle) _particlePool.Get().GetComponent(typeof(CustomParticle));
                    newParticle.lifeSpan = fruitBurst.lifeSpan;
                    newParticle.sprite = payoutSprite;
                    newParticle.transform.position = startingPosition;
                    newParticle.gameObject.SetActive(true);
                    randomStartingVelocity.Set(
                        Random.Range(fruitBurst.startVelocityMin.x , fruitBurst.startVelocityMax.x),
                        Random.Range(fruitBurst.startVelocityMin.y , fruitBurst.startVelocityMax.y)
                        );
                    newParticle.rigidBody2D.velocity = randomStartingVelocity;
                    newParticle.Init(_particlePool);
                }
                
                if (fruitBurst.cycles > 0)
                    currentCycle++;
                
                yield return intervalWait;
            }
        }
    }
}