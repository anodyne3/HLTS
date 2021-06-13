#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lwWVwuVXcOkbtz4sHvQEIuRMFc+Tb5182gdHFTOOruRYVOx8JIIJ6aLob4fyzngT12VTKMS+IuVk43fUMDx/eW5odXp1f31oeTxsc3B1f2VodHNudWhlLQosCBofSRgfDxFdbFliA1B3TIpdldhofhcMn12bL5adG/BhJZ+XTzzPJNito4ZTFnfjN+CtLETwRhgukHSvkwHCeW/je0J5oHgpPwlXCUUBr4jr6oCC00ym3URMFDcaHRkZGx4dCgJ0aGhsbyYzM2t1enV/fWh1c3I8XWlodHNudWhlLTJcuutbUWMUQiwDGh9JAT8YBCwKbHB5PE5zc2g8X10sAgsRLCosKC4aLBMaH0kBDx0d4xgZLB8dHeMsASqFUDFkq/GQh8Dva4fuas5rLFPdITp7PJYvdusRntPC978z5U92R3gsnhinLJ4fv7wfHh0eHh0eLBEaFWNdtITlzdZ6gDh3Dcy/p/gHNt8DGRwfnh0THCyeHRYenh0dHPiNtRU8fXJ4PH95bmh1enV/fWh1c3I8bHJ4PH9zcnh1aHVzcm88c3o8aW95CiwIGh9JGB8PEV1sbHB5PE5zc2gTgSHvN1U0BtTi0qmlEsVCAMrXIVXEaoMvCHm9a4jVMR4fHRwdv54dOP73zatswxNZ/TvW7XFk8fupCwtlPH1vb2lxeW88fX9/eWxofXJ/eSkuLSgsLypGCxEvKSwuLCUuLSgsfnB5PG9ofXJ4fW54PGh5bnFvPH2rB6GPXjgONtsTAapRgEJ/1FecCwONxwJbTPcZ8UJlmDH3Kr5LUEnwA5mfmQeFIVsr7rWHXJIwyK2MDsRufX9odX95PG9ofWh5cXlyaG8yLDxzejxodHk8aHR5cjx9bGxwdX99t79tjltPSd2zM12v5Of/bNH6v1BrazJ9bGxweTJ/c3EzfWxscHl/fZ4dHBoVNppUmut/eBkdLJ3uLDYanAg3zHVbiGoV4uh3kTJcuutbUWOJgmYQuFuXR8gKKy/X2BNR0gh1zUW7GRVgC1xKDQJoz6uXPydbv8lz3H8va+smGzBK98YTPRLGpm8FU6m0wGI+KdY5ycUTynfIvjg/Deu9sHB5PFVyfzItOiw4Gh9JGBcPAV1sxSpj3ZtJxbuFpS5e58TJbYJivU7VBW7pQRLJY0OH7jkfpkmTUUER7TMsnd8aFDcaHRkZGx4eLJ2qBp2vbHB5PF95bmh1enV/fWh1c3I8XWl7kxSoPOvXsDA8c2yqIx0skKtf0xgaDx5JTy0PLA0aH0kYFg8WXWxsqSax6BMSHI4XrT0KMmjJIBHHfgpmLJ4daiwSGh9JARMdHeMYGB8eHTosOBofSRgXDwFdbGxweTxfeW5oLA0aH0kYFg8WXWxscHk8VXJ/Mi1OeXB1fXJ/eTxzcjxodHVvPH95bjaaVJrrER0dGRkcLH4tFywVGh9JaHV6dX99aHk8fmU8fXJlPGx9bmgvKkYsfi0XLBUaH0kYGg8eSU8tDxofSQESGAoYCDfMdVuIahXi6HeRPF9dLJ4dPiwRGhU2mlSa6xEdHR0UQiyeHQ0aH0kBPBieHRQsnh0YLBEaFTaaVJrrER0dGRkcH54dHRxATLaWycb44MwVGyusaWk9");
        private static int[] order = new int[] { 16,55,50,44,5,25,51,26,20,35,11,31,51,29,48,56,16,17,48,45,43,51,22,27,53,40,26,42,59,56,49,46,41,53,36,36,41,48,52,57,54,44,58,45,48,54,51,50,58,59,51,58,52,55,57,57,59,59,59,59,60 };
        private static int key = 28;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
