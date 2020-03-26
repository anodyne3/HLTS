using System.Collections;
using Core.UI;
using MyScriptableObjects;
using UnityEngine;

namespace Core.CustomParticleGenerator
{
    public abstract class CustomParticle : GlobalAccess
    {
        public ParticleSettings particleSettings;
        
        [HideInInspector] public Rigidbody2D rigidBody2D;
        [HideInInspector] public Sprite sprite;
        [HideInInspector] public float lifeSpan;

        private MyObjectPool<CustomParticle> _particleObjectPool;

        public virtual void Awake()
        {
            rigidBody2D = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
            rigidBody2D.gravityScale = particleSettings.gravityModifier;

            var randomScale = Random.Range(particleSettings.startScaleMinMax.x, particleSettings.startScaleMinMax.y);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            transform.Rotate(
                Random.Range(particleSettings.startRotationMin.x, particleSettings.startRotationMax.x),
                Random.Range(particleSettings.startRotationMin.y, particleSettings.startRotationMax.y),
                Random.Range(particleSettings.startRotationMin.z, particleSettings.startRotationMax.z)
            );
        }

        public virtual void Init(MyObjectPool<CustomParticle> objectPool)
        {
            rigidBody2D.AddTorque(
                Random.Range(particleSettings.angularVelocityMinMax.x, particleSettings.angularVelocityMinMax.y),
                ForceMode2D.Impulse);
            _particleObjectPool = objectPool;
            StartCoroutine(nameof(LifeTime));
        }

        private IEnumerator LifeTime()
        {
            var life = lifeSpan;
            while (life > 0.0f)
            {
                life -= Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
            _particleObjectPool.Release(this);
        }
    }
}