using UnityEngine;

namespace Core.GameData
{
    public class GenericArrayDto
    {
        public readonly int[] newDataArray;

        public GenericArrayDto(string fromJson)
        {
            var jsonParsed = JsonUtility.FromJson<GenericArrayDto>(fromJson);
            newDataArray = jsonParsed.newDataArray;
        }
    }
}