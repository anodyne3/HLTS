using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.MainSlotMachine
{
    [Serializable]
    public class FruitParticle : GlobalAccess
    {
        public SpriteRenderer spriteRenderer;
        public Rigidbody2D rigidBody2D;
        public int orderInLayer;
        public float lifeSpan;
        public Vector2 scaleMinMax;
        public Vector3 minRotation;
        public Vector3 maxRotation;
        public float gravityModifier;
        public Vector2 angularVelocityMinMax;

        private void Awake()
        {
            spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            spriteRenderer.sortingOrder = orderInLayer;

            rigidBody2D = (Rigidbody2D) GetComponent(typeof(Rigidbody2D));
            rigidBody2D.gravityScale = gravityModifier;

            rigidBody2D.angularVelocity = Random.Range(angularVelocityMinMax.x, angularVelocityMinMax.y);

            var randomScale = Random.Range(scaleMinMax.x, scaleMinMax.y);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            transform.Rotate(
                Random.Range(minRotation.x, maxRotation.x),
                Random.Range(minRotation.y, maxRotation.y),
                Random.Range(minRotation.z, maxRotation.z)
            );
        }

        private void Start(){
            StartCoroutine(nameof(LifeTime));
        }

        private IEnumerator LifeTime()
        {
            var waitForEndOfFrame = new WaitForEndOfFrame();
            
            while (lifeSpan > 0.0f)
            {
                lifeSpan -= Time.deltaTime;
                yield return waitForEndOfFrame;
            }
            
            //gameObject.SetActive(false);
            ObjectPoolManager.fruitBurstPool.Release(this);
            yield return null;
        }
    }
}