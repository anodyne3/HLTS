using System.Collections.Generic;
using UnityEngine;

namespace Core.GameData
{
    public class RollDataDto
    {
        public int cr;
        public List<int> lr;
        public List<int> nr;
        public int np;

        public RollDataDto(string fromJson)
        {
            var jsonParsed = JsonUtility.FromJson<RollDataDto>(fromJson);
            cr = jsonParsed.cr;
            lr = jsonParsed.lr;
            nr = jsonParsed.nr;
            np = jsonParsed.np;
        }
    }
}