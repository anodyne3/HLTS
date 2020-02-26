using System;
using System.Collections.Concurrent;

namespace Core.UI
{
    [Serializable]
    public class MyObjectPool <T>
    {
        private readonly ConcurrentBag<T> _objects;
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
    }
}