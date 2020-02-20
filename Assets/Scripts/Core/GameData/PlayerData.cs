using Firebase.Auth;

namespace Core.GameData
{
    public class PlayerData : Singleton<PlayerData>
    {
        public FirebaseUser firebaseUser;
        public int CoinsAmount;
    }
}