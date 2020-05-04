using System;
using Enums;
using UnityEngine;

namespace Core.UI
{
    [Serializable]
    public class Resource
    {
        [HideInInspector] public long resourceAmount;
        public ResourceType resourceType;

        public Resource(long amount, ResourceType type)
        {
            resourceAmount = amount;
            resourceType = type;
        }
    }
}