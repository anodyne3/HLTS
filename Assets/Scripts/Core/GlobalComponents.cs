using System;
using Core.GameData;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace Core
{
    public class GlobalComponents : Singleton<GlobalComponents>
    {
        //private Dictionary<string, Component> _Components = new Dictionary<string, Component>();
        private List<Component> _components = new List<Component>();

        protected GlobalComponents()
        {
        }

        private PlayerData _playerData = new GameObject().AddComponent<PlayerData>();

        public PlayerData PlayerData2()
        {
            return _playerData;
        }

        public Component AddGlobalComponent(Type type)
        {
            var existingComponent = GetGlobalComponent(type);

            if (existingComponent != null)
            {
                Debug.LogWarning("[Toolbox] Global component ID \"" + type + "\" already exist! Returning that.");
                return existingComponent;
            }

            var newComponent = gameObject.AddComponent(type);
            _components.Add(newComponent);
            return newComponent;
        }

        public void RemoveGlobalComponent(Type type)
        {
            var existingComponent = GetGlobalComponent(type);
            
            if (existingComponent != null)
            {
                Destroy(existingComponent);
                _components.Remove(existingComponent);
            }
            else
                Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \"" + type + "\"! Typo?");
        }

        private Component GetGlobalComponent(Type type)
        {
            var existingComponent = new Component();
            
            if (_components.Any(x => x.TryGetComponent(type, out existingComponent)))
                return existingComponent;

            Debug.LogWarning("[Toolbox] Global component ID \"" + type + "\" doesn't exist! Typo?");
            return null;
        }
    }
}