using System.Collections.Generic;
using UnityEngine;

namespace Core.GameData
{
    public class PlayerDataDto
    {
        public int coinsAmount;
        public List<int> lastResult;
        public List<int> nextResult;

        public PlayerDataDto(string fromJson)
        {
            var jsonParsed = JsonUtility.FromJson<PlayerDataDto>(fromJson);
            coinsAmount = jsonParsed.coinsAmount;
            lastResult = jsonParsed.lastResult;
            nextResult = jsonParsed.nextResult;
        }
    }
}