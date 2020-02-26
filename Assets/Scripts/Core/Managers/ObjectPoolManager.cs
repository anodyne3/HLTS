using Core.UI;

namespace Core.Managers
{
    public class ObjectPoolManager : GlobalAccess
    {
        private CoinDragHandler _coinPrefab;
        
        public MyObjectPool<CoinDragHandler> coinPool;

        private void Awake()
        {
            Foundation.ObjectPoolManager = this;
            
            DontDestroyOnLoad(gameObject);
        }
    }
}