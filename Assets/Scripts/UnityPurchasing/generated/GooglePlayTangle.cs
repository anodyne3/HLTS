#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mCqpipilrqGCLuAuX6WpqamtqKvh14Aftuu2C+hiEGiESeifhyw16unvNgHSFI1oasoIL8/4NWrkmnSa9/ELDUJSSiNFZo9D33pPL+qE8O20cWDT7nuOjvQfiaotTETTs9lP4ldPeSruCb8mlMk1oS+2gCGinVR3CfdTJNkEiiQwq1w2gZ4pTeXrY5+JohSOVXapC6qX4AkdFvSe4e4KKyqpp6iYKqmiqiqpqahkqTnQtbjQQqGWjnZa5GdvafrNrmYSLKOS93yepLpR8NIDUdB6vkqg1NP3TdD855B/0DEk7AWiRM6TkouLuC/7TmZUNjT9P84OODHPxvgtZ5+jwsjkl/vvEFEDOHOQWZMH6l/i+cWDABDjugWxSvm2ws9yc6qrqaip");
        private static int[] order = new int[] { 0,11,12,13,5,8,9,8,11,9,12,13,12,13,14 };
        private static int key = 168;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
