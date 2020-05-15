using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class GeneralUtils
    {
        public static T[] SortLoadedList<T>(string path, Comparison<T> comparison) where T : ScriptableObject
        {
            var loadedList = new List<T>();
            foreach (var o in Resources.LoadAll<T>(path)) loadedList.Add(o);
            loadedList.Sort(comparison);
            return loadedList.ToArray();
        }
    }
}