using UnityEngine;

namespace Core.Managers
{
    public class CameraManager : GlobalClass//Singleton<CameraManager>
    {
        private Camera _mainCamera;
        public Camera MainCamera => _mainCamera != null ? _mainCamera : Camera.main;

        public override void Awake()
        {
            base.Awake();
            
            _mainCamera = Camera.main;
        }
    }
}