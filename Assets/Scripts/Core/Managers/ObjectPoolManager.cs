using Core.MainSlotMachine;
using Core.UI;

namespace Core.Managers
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>//GlobalAccess
    {
        public MyObjectPool<CoinDragHandler> coinPool;
        public MyObjectPool<FruitParticle> fruitBurstPool;

        /*private void OnGUI()
        {
            GUI.Box(new Rect(10,10,100,50), "ObjectsCount:");
            if (fruitBurstPool == null) return;
            GUI.Box(new Rect(10,30,100,30), fruitBurstPool._objects.Count.ToString());
        }*/
    }
}