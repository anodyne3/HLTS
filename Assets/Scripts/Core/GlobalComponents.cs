using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Core.GameData;
using Core.Managers;

namespace Core
{
    public class GlobalComponents : Singleton<GlobalComponents>
    {
        private readonly List<Component> _components = new List<Component>();
        private bool _shuttingDown;

        protected GlobalComponents()
        {
        }

        private void Awake()
        {
            if (GameManager != null && GameManager.debug)
                _components.Add(gameObject.AddComponent<AdManager>());
            
            _components.Add(gameObject.AddComponent<FirebaseFunctionality>());
            _components.Add(gameObject.AddComponent<PlayerData>());
            _components.Add(gameObject.AddComponent<SlotMachine>());
        }

        public Component AddGlobalComponent<T>() where T : GlobalAccess
        {
            if (_shuttingDown)
                return null;
            
            var existingComponent = GetGlobalComponent<T>();

            if (existingComponent != null)
                return existingComponent;

            var newComponent = gameObject.AddComponent<T>();
            _components.Add(newComponent);
            return newComponent;
        }

        public void RemoveGlobalComponent<T>() where T : GlobalAccess
        {
            var existingComponent = GetGlobalComponent<T>();

            if (existingComponent == null) return;

            Destroy(existingComponent);
            _components.Remove(existingComponent);
        }

        private Component GetGlobalComponent<T>() where T : GlobalAccess
        {
            var existingComponent = new Component();

            return _components.Any(x => x.TryGetComponent(typeof(T), out existingComponent)) ? existingComponent : null;
        }
        
        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        private void OnDestroy()
        {
            _shuttingDown = true;
        }
    }
}