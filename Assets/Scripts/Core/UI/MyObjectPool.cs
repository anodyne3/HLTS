using System;
using System.Collections.Concurrent;

namespace Core.UI
{
    public class MyObjectPool <T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        public MyObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) return;
        
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T Get()
        {
            return _objects.TryTake(out var item) ? item : _objectGenerator();
        }

        public void Release(T item)
        {
            _objects.Add(item);
        }

        public int PoolCount()
        {
            return _objects.Count;
        }
    }
}