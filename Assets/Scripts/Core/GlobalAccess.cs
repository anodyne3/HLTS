using Core.Managers;
using UnityEngine;

namespace Core
{
    public class GlobalAccess : MonoBehaviour
    {
        protected static CameraManager CameraManager => Foundation.GetAssignedClass<CameraManager>();
        // protected AudioManager AudioManager => Foundation.GetAssignedClass<AudioManager>();
        protected AudioManager AudioManager => gameObject.AddComponent<AudioManager>();
    }
}