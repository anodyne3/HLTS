using Core.MainSlotMachine;
using Core.UI;

namespace Core.Managers
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>//GlobalAccess
    {
        //private CoinDragHandler _coinPrefab;
        //private FruitParticle _fruitBurstPrefab;
        
        public MyObjectPool<CoinDragHandler> coinPool;
        public MyObjectPool<FruitParticle> fruitBurstPool;

        private void Awake()
        {
            /*Foundation.ObjectPoolManager = this;
            
            DontDestroyOnLoad(gameObject);*/
        }
    }
}