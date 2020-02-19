using Firebase.Database;

namespace Core
{
    public class DatabaseFunctions : Singleton
    {
        private static DatabaseReference DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        /*public void WriteNewScore(string userId, int score)
        {
            // Create new entry at /user-scores/$userid/$scoreid and at /scores/$scoreid simultaneously
            var key = DatabaseReference.Child("scores").Push().Key;
            var entry = new LeaderBoardEntry(userId, score);
            var entryValues = entry.ToDictionary();

            var childUpdates = new Dictionary<string, object>
            {
                ["/scores/" + key] = entryValues, ["/user-scores/" + userId + "/" + key] = entryValues
            };

            DatabaseReference.UpdateChildrenAsync(childUpdates);
        }*/

        public static void UserReset()
        {
            DatabaseReference.Child("users").Child(GameData.GameData.Instance.firebaseUser.UserId).Child("userData")
                .Child("userWrites").RemoveValueAsync();
        }
    }

    /*public class LeaderBoardEntry
    {
        private readonly string _uid;
        private readonly int _score;

        public LeaderBoardEntry(string uid, int score)
        {
            _uid = uid;
            _score = score;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object> {["uid"] = _uid, ["score"] = _score};

            return result;
        }
    }*/
}