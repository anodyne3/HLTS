using System;
using Core.UI;
using UnityEngine;

namespace Core.CustomParticleGenerator
{
    [Serializable]
    public class SpriteRendererParticle : CustomParticle
    {
        public SpriteRenderer spriteRenderer;
        public int orderInLayer;

        public override void Awake()
        {
            base.Awake();
            
            spriteRenderer = (SpriteRenderer) GetComponent(typeof(SpriteRenderer));
            spriteRenderer.sortingOrder = orderInLayer;
        }

        public override void Init(MyObjectPool<CustomParticle> ObjectPool)
        {
            base.Init(ObjectPool);

            spriteRenderer.sortingOrder += ObjectPool.PoolCount();
            spriteRenderer.sprite = sprite;
        }
    }
}