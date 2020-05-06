using Newtonsoft.Json;

namespace Core.GameData
{
    public class GenericArrayDto
    {
        public readonly int[] newDataArray;

        public GenericArrayDto(string fromJson)
        {
            // if (isArray)
            {
                var jsonParsed = JsonConvert.DeserializeObject<int[]>(fromJson);
                newDataArray = jsonParsed;
            }
            // else
            {
                // var jsonParsed = JsonUtility.FromJson<GenericArrayDto>(fromJson);
                // newDataArray = jsonParsed.newDataArray;
            }
        }
    }
}