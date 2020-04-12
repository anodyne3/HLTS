using MyScriptableObjects;
using UnityEngine;
using Utils;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Core.Managers
{
    public class CameraDragHandler : GlobalAccess
    {
        [SerializeField] private CameraSettings mainCameraOffsets;

        private bool _cameraChanged;
        private float _zoomPercent;
        private float _zoomCurrent;
        private float _adjustedPanMultiplier;
        private float _adjustedZoomMultiplier;
        private float _adjustedPanRange;
        private float _zoomTotalRange;
        private float NewOrthographicSize { get; set; }
        private Vector3 _newLocalPosition;
        
        //android properties
        private Vector2 _pinchPositionDelta;
        private float _pinchZoom;
        private float _deltaMagnitudeDiff;
        private bool _isPinching;

        private void Start()
        {
            NewOrthographicSize = CameraManager.MainCamera.orthographicSize;

            InputManager.Scrolled += OnScrolled;
            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
            InputManager.Pinched += PinchZoom;

            _zoomTotalRange = mainCameraOffsets.zoomMax - mainCameraOffsets.zoomMin;
        }

        private void OnDisable()
        {
            if (InputManager == null) return;
            
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

        private void OnPressed(Vector2 pointerPosition)
        {
            _isPinching = false;
            
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
            if (_isPinching) return;
            
            if (GameManager != null)
                GameManager.interactionEnabled = false;

            _newLocalPosition.x += dragData.x * _adjustedPanMultiplier;
            _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x, -_adjustedPanRange, _adjustedPanRange);
            _newLocalPosition.y += dragData.y * _adjustedPanMultiplier;
            _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y, -_adjustedPanRange, _adjustedPanRange);

            if ((transform.localPosition - _newLocalPosition).sqrMagnitude > Constants.WorldSpaceTolerance)
                _cameraChanged = true;
        }

        private void CenterAsZoom(float zoomData)
        {
            _adjustedZoomMultiplier = _zoomPercent > 0.0f
                ? mainCameraOffsets.zoomMultiplier * (1.0f - _zoomPercent)
                : mainCameraOffsets.zoomMultiplier;
            NewOrthographicSize = CameraManager.MainCamera.orthographicSize - zoomData * _adjustedZoomMultiplier;
            NewOrthographicSize = Mathf.Clamp(NewOrthographicSize, mainCameraOffsets.zoomMin, mainCameraOffsets.zoomMax);

            _zoomCurrent = mainCameraOffsets.zoomMax - NewOrthographicSize;
            _zoomPercent = _zoomCurrent / _zoomTotalRange;

            _adjustedPanMultiplier = _zoomPercent > 0.0f
                ? mainCameraOffsets.panMultiplier * (1.0f - _zoomPercent)
                : mainCameraOffsets.panMultiplier;

            _adjustedPanRange = mainCameraOffsets.panRange * _zoomPercent;

            if (zoomData < 0.0f)
            {
                //Debug.Log("zP < 0"); //ZoomOut
                _newLocalPosition.x *= _zoomPercent;
                _newLocalPosition.y *= _zoomPercent;
            }

            if (CameraManager == null) return;

            if (Mathf.Abs(CameraManager.MainCamera.orthographicSize - NewOrthographicSize) > Constants.FloatTolerance)
                _cameraChanged = true;
        }

        private void CameraUpdate()
        {
            if (!_cameraChanged) return;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition, _newLocalPosition, Time.deltaTime * mainCameraOffsets.panLerpSpeed);

            CameraManager.MainCamera.orthographicSize = Mathf.Lerp(CameraManager.MainCamera.orthographicSize,
                NewOrthographicSize, Time.deltaTime * mainCameraOffsets.zoomLerpSpeed);

            if (Mathf.Abs(CameraManager.MainCamera.orthographicSize - NewOrthographicSize) < Constants.FloatTolerance &&
                (transform.localPosition - _newLocalPosition).sqrMagnitude < Constants.WorldSpaceTolerance)
                _cameraChanged = false;
        }

        private void PinchZoom(Touch touchZero, Touch touchOne)
        {
            _isPinching = false;
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
                _isPinching = true;
            }
        }
    }
}