using Core.Managers;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Core
{
    public class FirebaseCheckAndFixDependencies : GlobalAccess
    {
        public FirebaseApp firebaseApp;
        private static FirebaseAuth _firebaseAuth;
        public bool firebaseReady;

        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    firebaseApp = FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    firebaseReady = true;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        
            firebaseApp.SetEditorDatabaseUrl("https://he-loves-the-slots.firebaseio.com/");
        
            Login();

            GameManager.LoadMain();
        }

        public static void Login()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        
            _firebaseAuth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was cancelled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                var newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                GameData.GameData.Instance.firebaseUser = newUser;
            });
        }
    }
}