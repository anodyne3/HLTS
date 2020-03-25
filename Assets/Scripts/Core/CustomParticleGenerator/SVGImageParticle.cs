using Core.UI;

namespace Core.CustomParticleGenerator
{
    public class SVGImageParticle : CustomParticle
    {
        public SVGImage svgImage;

        public override void Awake()
        {
            base.Awake();
            
            svgImage = (SVGImage) GetComponent(typeof(SVGImage));
        }

        public override void Init(MyObjectPool<CustomParticle> ObjectPool)
        {
            base.Init(ObjectPool);

            svgImage.sprite = sprite;
        }
    }
}