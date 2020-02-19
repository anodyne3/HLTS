using UnityEngine;

namespace Core.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera != null ? _mainCamera : Camera.main;

        public void Awake()
        {
            _mainCamera = Camera.main;
        }
    }
}