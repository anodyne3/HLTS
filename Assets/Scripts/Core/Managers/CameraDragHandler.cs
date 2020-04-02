using System;
using MyScriptableObjects;
using UnityEngine;
using Utils;

namespace Core.Managers
{
    public class CameraDragHandler : InputManager
    {
        [SerializeField] private CameraOffsets mainCameraOffsets;
        private bool _cameraChanged;
        private float _zoomPercentPan;
        private float _zoomPercentInverse;

        private const float DragRateTop = 0.01f; //reduced from 0.015f
        private float _dpiCompensate = 1.0f;
        private const float CazRate = 0.4f;

        private float NewOrthographicSize { get; set; }
        private Vector3 _newLocalPosition;

#if UNITY_ANDROID
        private Vector3 _pinchPosition;
        private Vector2 _pinchPositionDelta;
        private const float PinchRate = 0.08f;
        private float _pinchZoom;
        private const float PinchIgnore = 1.25f;
        private float _deltaMagnitudeDiff;
#endif

        private void Start()
        {
            NewOrthographicSize = CameraManager.MainCamera.orthographicSize;
        }

        private void FixedUpdate()
        {
            Dragging();
        }

        protected override void OnDragBegin(Vector2 inputPos)
        {
            base.OnDragBegin(inputPos);
        }

        protected override void OnDragging(Vector2 inputPos)
        {
            base.OnDragging(inputPos);

            if (CameraManager.draggingDisabled) return;

            CameraPanning(inputPos);
        }

        public override void OnDragEnd()
        {
            base.OnDragEnd();

            if (GameManager == null) return;
            
            GameManager.interactionEnabled = true;
        }

        private void Update()
            /*private void UpdateCamera()*/
        {
#if UNITY_EDITOR
            if (Math.Abs(Input.mouseScrollDelta.y) > Constants.FloatTolerance)
            {
                CenterAsZoom(Input.mouseScrollDelta.y);
            }
#elif UNITY_ANDROID
		    PinchZoom();
#endif
            CameraUpdate();
        }

        private void CameraPanning(Vector2 dragData)
        {
            // GameManager.interactionEnabled = false;

            var position = transform.position;
            _zoomPercentPan = (mainCameraOffsets.zoomMax - position.y) /
                              (mainCameraOffsets.zoomMax - mainCameraOffsets.zoomMin);
            const float floatTolerance = 0.001f;
            if (Math.Abs(mainCameraOffsets.zoomMax - position.y) < floatTolerance)
            {
                _zoomPercentInverse = 0;
            }
            else
            {
                _zoomPercentInverse =
                    (mainCameraOffsets.zoomMax - mainCameraOffsets.zoomMin) / (mainCameraOffsets.zoomMax - position.y);
            }

            mainCameraOffsets.pivotLerpSpeed = 15.0f; //changed from 25.0f
            var pivoterX = dragData.x * DragRateTop * _zoomPercentInverse * _dpiCompensate;

            _newLocalPosition.x -= pivoterX;
            _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x,
                mainCameraOffsets.pivoterBase.x - mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.pivoterBase.x + mainCameraOffsets.panRange * _zoomPercentPan);
            _newLocalPosition.y -= dragData.y * DragRateTop * _zoomPercentInverse * _dpiCompensate;
            _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y,
                mainCameraOffsets.pivoterBase.y - mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.pivoterBase.y + mainCameraOffsets.panRange * _zoomPercentPan);

            _cameraChanged = true;
        }

        private void CenterAsZoom(float zoomData)
        {
            NewOrthographicSize = CameraManager.MainCamera.orthographicSize - zoomData * mainCameraOffsets.zoomMultiplier;
            NewOrthographicSize = Mathf.Clamp(NewOrthographicSize,
                mainCameraOffsets.zoomMin, mainCameraOffsets.zoomMax);
            _zoomPercentPan = (mainCameraOffsets.zoomMax - NewOrthographicSize) *
                              (1 / (float.IsNaN(mainCameraOffsets.zoomMin) ? 1 : mainCameraOffsets.zoomMin));
            _newLocalPosition = transform.localPosition;
            if (zoomData < 0.0f)
            {
                //Debug.Log("zP < 0"); //ZoomOut
                _newLocalPosition.x -= (mainCameraOffsets.pivoterBase.x - _newLocalPosition.x) *
                                       _zoomPercentPan * 0.1f;
                _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x,
                    mainCameraOffsets.pivoterBase.x - mainCameraOffsets.panRange * _zoomPercentPan,
                    mainCameraOffsets.pivoterBase.x + mainCameraOffsets.panRange * _zoomPercentPan);
                _newLocalPosition.y -= (mainCameraOffsets.pivoterBase.y - _newLocalPosition.y) *
                                       _zoomPercentPan * 0.1f;
                _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y,
                    mainCameraOffsets.pivoterBase.y - mainCameraOffsets.panRange * _zoomPercentPan,
                    mainCameraOffsets.pivoterBase.y + mainCameraOffsets.panRange * _zoomPercentPan);
            }
            else
            {
                //Debug.Log("zP > 0"); //ZoomIn
                _newLocalPosition.x += Screen.width * 0.5f / Screen.width * CameraManager.MainCamera.aspect * CazRate;
                _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x,
                    mainCameraOffsets.pivoterBase.x - mainCameraOffsets.panRange,
                    mainCameraOffsets.pivoterBase.x + mainCameraOffsets.panRange);
                _newLocalPosition.y += Screen.height * 0.5f / Screen.height * CazRate;
                _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y,
                    mainCameraOffsets.pivoterBase.y - mainCameraOffsets.panRange,
                    mainCameraOffsets.pivoterBase.y + mainCameraOffsets.panRange);
            }

            _cameraChanged = true;
        }

        private void CameraUpdate()
        {
            if (!_cameraChanged) return;

            transform.position = Vector3.Lerp(
                transform.localPosition, _newLocalPosition, Time.deltaTime * mainCameraOffsets.pivotLerpSpeed);

            CameraManager.MainCamera.orthographicSize = Mathf.Lerp(CameraManager.MainCamera.orthographicSize,
                NewOrthographicSize, Time.deltaTime * mainCameraOffsets.zoomLerpSpeed);

            if (CameraManager.MainCamera.orthographicSize - NewOrthographicSize < Constants.FloatTolerance &&
                (transform.localPosition - _newLocalPosition).sqrMagnitude < Constants.FloatTolerance)
                _cameraChanged = false;
        }

        private void CameraPanning(Vector2 dragData, CameraOffsets cameraOffsets)
        {
            var position = transform.localPosition;
            _zoomPercentPan = (cameraOffsets.zoomMax - position.y) / (cameraOffsets.zoomMax - cameraOffsets.zoomMin);
            if (Math.Abs(cameraOffsets.zoomMax - position.y) < Constants.FloatTolerance)
            {
                _zoomPercentInverse = 0;
            }
            else
            {
                _zoomPercentInverse =
                    (cameraOffsets.zoomMax - cameraOffsets.zoomMin) / (cameraOffsets.zoomMax - position.y);
            }

            var pivoterX = dragData.x * DragRateTop * _zoomPercentInverse * _dpiCompensate;

            _newLocalPosition.x -= pivoterX;
            _newLocalPosition.x = Mathf.Clamp(_newLocalPosition.x,
                mainCameraOffsets.pivoterBase.x - mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.pivoterBase.x + mainCameraOffsets.panRange * _zoomPercentPan);
            _newLocalPosition.y -= dragData.y * DragRateTop * _zoomPercentInverse * _dpiCompensate;
            _newLocalPosition.y = Mathf.Clamp(_newLocalPosition.y,
                mainCameraOffsets.pivoterBase.y - mainCameraOffsets.panRange * _zoomPercentPan,
                mainCameraOffsets.pivoterBase.y + mainCameraOffsets.panRange * _zoomPercentPan);
        }

#if UNITY_ANDROID
        private void PinchZoom()
        {
            if (Input.touchCount != 2) return;

            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            _pinchPositionDelta = (touchZero.deltaPosition + touchOne.deltaPosition) / 2.0f;

            // _pinchPosition = (touchZero.position + touchOne.position) / 2.0f;
            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            _deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
            _pinchZoom = _deltaMagnitudeDiff * PinchRate;

            if (Mathf.Abs(_pinchZoom) < PinchIgnore)
            {
                CameraPanning(_pinchPositionDelta, mainCameraOffsets);
            }
            else if (Mathf.Abs(_pinchZoom) > PinchIgnore)
            {
                CenterAsZoom(_pinchZoom);
            }
        }
#endif
    }
}