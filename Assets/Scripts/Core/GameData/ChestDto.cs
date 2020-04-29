using UnityEngine;

namespace Core.GameData
{
    public class ChestDto
    {
        public readonly int[] newChestData;

        public ChestDto(string fromJson)
        {
            var jsonParsed = JsonUtility.FromJson<ChestDto>(fromJson);
            newChestData = jsonParsed.newChestData;
        }
    }
}