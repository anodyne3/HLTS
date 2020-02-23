using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Core.GameData;

namespace Core
{
    public class GlobalComponents : Singleton<GlobalComponents>
    {
        //private Dictionary<string, Component> _Components = new Dictionary<string, Component>();
        private List<Component> _components = new List<Component>();

        protected GlobalComponents()
        {
        }

        /*
        private PlayerData _playerData2;

        public PlayerData PlayerData2()
        {
            return _playerData2;
        }*/

        private void Awake()
        {
            // _playerData2 = new GameObject().AddComponent<PlayerData>();
            // _components.Add(_playerData2);
        }

        public Component AddGlobalComponent<T>() where T : GlobalAccess
        {
            var existingComponent = GetGlobalComponent<T>();

            if (existingComponent != null)
            {
                //Debug.LogWarning("[Toolbox] Global component ID \"" + typeof(T) + "\" already exist! Returning that.");
                return existingComponent;
            }

            var newComponent = gameObject.AddComponent<T>();
            _components.Add(newComponent);
            return newComponent;
        }

        public void RemoveGlobalComponent<T>() where T : GlobalAccess
        {
            var existingComponent = GetGlobalComponent<T>();
            
            if (existingComponent != null)
            {
                Destroy(existingComponent);
                _components.Remove(existingComponent);
            }
            // else Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \"" + typeof(T) + "\"! Typo?");
        }

        private Component GetGlobalComponent<T>() where T : GlobalAccess
        {
            var existingComponent = new Component();
            
            if (_components.Any(x => x.TryGetComponent(typeof(T), out existingComponent)))
                return existingComponent;

            // Debug.LogWarning("[Toolbox] Global component ID \"" + typeof(T) + "\" doesn't exist! Typo?");
            
            return null;
        }
    }
}