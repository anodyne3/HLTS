using Core.UI;
using UnityEngine;

namespace Core.Managers
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        public MyObjectPool<CoinDragHandler> coinPool;

        public static MyObjectPool<T> CreateObjectPool<T>(Object prefab, Transform parent) where T : Object
        {
            var newPool = new MyObjectPool<T>(() => Instantiate((T)prefab, parent));
            return newPool;
        }
        
        /*private void OnGUI()
        {
            GUI.Box(new Rect(10,10,100,50), "ObjectsCount:");
            if (fruitBurstPool == null) return;
            GUI.Box(new Rect(10,30,100,30), fruitBurstPool._objects.Count.ToString());
        }*/
    }
}