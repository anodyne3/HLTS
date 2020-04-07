using System;
using MyScriptableObjects;
using UnityEngine;
using Utils;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Core.Managers
{
    public class CameraDragHandler : GlobalAccess
    {
        [SerializeField] private CameraSettings mainCameraOffsets;
        [SerializeField] private bool _cameraChanged;
        [SerializeField] private float _zoomPercentPan;
        [SerializeField] private float _zoomPercentInverse;

        private float NewOrthographicSize { get; set; }
        private Vector3 _newLocalPosition;

#if UNITY_ANDROID
        private Vector2 _pinchPositionDelta;
        private float _pinchZoom;
        private float _deltaMagnitudeDiff;
#endif

        private void Start()
        {
            NewOrthographicSize = CameraManager.MainCamera.orthographicSize;

            InputManager.Scrolled += OnScrolled;
            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
            InputManager.Pinched += PinchZoom;
        }

        private void OnDisable()
        {
            InputManager.Scrolled -= OnScrolled;
            InputManager.Pressed -= OnPressed;
            InputManager.Dragged -= OnDragged;
            InputManager.Released -= OnReleased;
            InputManager.Pinched -= PinchZoom;
        }

        private void OnScrolled(float scrollAmount)
        {
            CenterAsZoom(scrollAmount);
        }

        private static void OnPressed(Vector2 pointerPosition)
        {
            if (CameraManager.draggingDisabled) return;

            InputManager.startPosition = pointerPosition;
        }

        private void OnDragged(Vector2 pointerPosition)
        {
            if (CameraManager.draggingDisabled) return;

            InputManager.dragDelta = InputManager.startPosition - pointerPosition;
            InputManager.startPosition = pointerPosition;

            CameraPanning(InputManager.dragDelta);
        }

        private static void OnReleased()
        {
            if (GameManager == null) return;

            GameManager.interactionEnabled = true;
        }

        private void Update()
        {
            CameraUpdate();
        }

        private void CameraPanning(Vector2 dragData)
        {
            // GameManager.interactionEnabled = false;

            var zoomMaxLessZoom = mainCameraOffsets.zoomMax - CameraManager.MainCamera.orthographicSize;
            var zoomTotalRange = mainCameraOffsets.zoomMax - mainCameraOffsets.zoomMin;

            _zoomPercentPan = zoomMaxLessZoom / zoomTotalRange;

            if (Math.Abs(zoomMaxLessZoom) < Constants.FloatTolerance)
                _zoomPercentInverse = 0;
            else
                _zoomPercentInverse = zoomTotalRange / zoomMaxLessZoom;

            _newLocalPosition.x += dragData.x * mainCameraOffsets.dragRateTop * _zoomPercentInverse;
            _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x, -mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.panRange * _zoomPercentPan);
            _newLocalPosition.y += dragData.y * mainCameraOffsets.dragRateTop * _zoomPercentInverse;
            _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y, -mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.panRange * _zoomPercentPan);

            if ((transform.localPosition - _newLocalPosition).sqrMagnitude > Constants.FloatTolerance)
                _cameraChanged = true;
        }

        private void CenterAsZoom(float zoomData)
        {
            NewOrthographicSize =
                CameraManager.MainCamera.orthographicSize - zoomData * mainCameraOffsets.zoomMultiplier;
            NewOrthographicSize = Mathf.Clamp(NewOrthographicSize,
                mainCameraOffsets.zoomMin, mainCameraOffsets.zoomMax);
            _zoomPercentPan = (mainCameraOffsets.zoomMax - NewOrthographicSize) *
                              (1 / (float.IsNaN(mainCameraOffsets.zoomMin) ? 1 : mainCameraOffsets.zoomMin));
            _newLocalPosition = transform.localPosition;
            if (zoomData < 0.0f)
            {
                //Debug.Log("zP < 0"); //ZoomOut
                _newLocalPosition.x -= _newLocalPosition.x * _zoomPercentPan * mainCameraOffsets.panMultiplier;
                _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x, -mainCameraOffsets.panRange * _zoomPercentPan,
                    mainCameraOffsets.panRange * _zoomPercentPan);
                _newLocalPosition.y -= _newLocalPosition.y * _zoomPercentPan * mainCameraOffsets.panMultiplier;
                _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y,
                    -mainCameraOffsets.panRange * _zoomPercentPan,
                    mainCameraOffsets.panRange * _zoomPercentPan);
            }
            else
            {
                //Debug.Log("zP > 0"); //ZoomIn
                _newLocalPosition.x += Screen.width * 0.5f / Screen.width * CameraManager.MainCamera.aspect *
                                       mainCameraOffsets.cazRate;
                _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x, -mainCameraOffsets.panRange,
                    mainCameraOffsets.panRange);
                _newLocalPosition.y += Screen.height * 0.5f / Screen.height * mainCameraOffsets.cazRate;
                _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y, -mainCameraOffsets.panRange,
                    mainCameraOffsets.panRange);
            }

            if (Mathf.Abs(CameraManager.MainCamera.orthographicSize - NewOrthographicSize) > Constants.FloatTolerance)
                _cameraChanged = true;
        }

        private void CameraUpdate()
        {
            if (!_cameraChanged) return;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition, _newLocalPosition, Time.deltaTime * mainCameraOffsets.pivotLerpSpeed);

            CameraManager.MainCamera.orthographicSize = Mathf.Lerp(CameraManager.MainCamera.orthographicSize,
                NewOrthographicSize, Time.deltaTime * mainCameraOffsets.zoomLerpSpeed);

            if (Mathf.Abs(CameraManager.MainCamera.orthographicSize - NewOrthographicSize) < Constants.FloatTolerance &&
                (transform.localPosition - _newLocalPosition).sqrMagnitude < Constants.FloatTolerance)
            {
                _cameraChanged = false;
            }
        }

        private void PinchZoom(Touch touchZero, Touch touchOne)
        {
            _pinchPositionDelta = (touchZero.delta + touchOne.delta) * 0.5f;

            var touchZeroPrevPos = touchZero.screenPosition - touchZero.delta;
            var touchOnePrevPos = touchOne.screenPosition - touchOne.delta;

            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var touchDeltaMag = (touchZero.screenPosition - touchOne.screenPosition).magnitude;
            _deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
            _pinchZoom = _deltaMagnitudeDiff * mainCameraOffsets.pinchRate;

            if (Mathf.Abs(_pinchZoom) < mainCameraOffsets.pinchIgnore)
            {
                CameraPanning(_pinchPositionDelta);
            }
            else if (Mathf.Abs(_pinchZoom) > mainCameraOffsets.pinchIgnore)
            {
                CenterAsZoom(_pinchZoom);
            }
        }
    }
}