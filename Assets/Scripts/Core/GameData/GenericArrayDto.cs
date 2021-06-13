using Newtonsoft.Json;

namespace Core.GameData
{
    public class GenericArrayDto
    {
        public readonly int[] newDataArray;

        public GenericArrayDto(string fromJson)
        {
            var jsonParsed = JsonConvert.DeserializeObject<int[]>(fromJson);
            newDataArray = jsonParsed;
        }
    }
}