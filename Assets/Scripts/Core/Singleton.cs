using System;
using UnityEngine;

namespace Core
{
    public abstract class Singleton : GlobalAccess
    {
        private static readonly Lazy<Singleton> Lazy = new Lazy<Singleton>(() => new GameObject("Singleton").AddComponent<Singleton>());

        protected static Singleton Instance => Lazy.Value;

        protected Singleton()
        {
        }
    }
}