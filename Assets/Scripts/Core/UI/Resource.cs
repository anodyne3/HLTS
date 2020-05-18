using System;
using Enums;

namespace Core.UI
{
    [Serializable]
    public class Resource
    {
        public long resourceAmount;
        public ResourceType resourceType;

        public Resource(long amount, ResourceType type)
        {
            resourceAmount = amount;
            resourceType = type;
        }
    }
}