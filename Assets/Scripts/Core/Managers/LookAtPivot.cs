using System;
using Core;
using Core.Managers;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LookAtPivot : GlobalAccess, IDragHandler
{
    private Camera _mainCamera => CameraManager.MainCamera;
    public bool topCamera = true;
    private bool _pivotSpin;
    private bool _winSpin;

    [SerializeField] private CameraOffsets _mainCameraOffsets;
    private Transform _pivot;
    private GameObject _pitch;
    private Vector3 _pitcher;
    private Vector3 _standardCamera;

    public bool scrollZoom;
    private float _zoomPercentPan;
    private float _zoomPercentPers;
    private float _zoomOffset;
    private float _pivotLerpSpeed;

    [SerializeField] private Image cameraButtonImage;

    private const float DragRateTop = 0.01f; //reduced from 0.015f
    private const float CazRate = 0.4f;
    private float _zoomPercentInverse;

    private float _dpiCompensate = 1.0f;

#if UNITY_ANDROID
    private Vector3 _pinchPosition;
    private Vector2 _pinchPositionDelta;
    private const float PinchRate = 0.08f;
    private float _pinchZoom;
    private const float PinchIgnore = 1.25f;
    private float _deltaMagnitudeDiff;

#endif

    private void Awake()
    {
        _pitch = transform.parent.gameObject;
        _pivot = _pitch.transform.parent.transform;
        _standardCamera = _pitch.transform.rotation.eulerAngles;
    }

    private void Start()
    {
        // _pivot.position = _mainCameraOffsets.pivoter;
#if UNITY_ANDROID
        _dpiCompensate = 1000.0f / Mathf.Max(Screen.width, Screen.height);
#endif
        CameraManager.MainCamera.GetComponent<AudioSource>().mute = false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.mouseScrollDelta.y != 0.0f)
        {
            CenterAsZoom(Input.mouseScrollDelta.y, Input.mousePosition, _mainCameraOffsets);
        }
#elif UNITY_ANDROID
		PinchZoom();
#endif
        CameraUpdate();
    }

    private void CameraUpdate()
    {
        if (topCamera)
        {
            /*_pivot.position = Vector3.Lerp(_pivot.position, _mainCameraOffsets.pivoter,
                Time.deltaTime * _pivotLerpSpeed);*/

            _pitch.transform.rotation = Quaternion.Lerp(_pitch.transform.rotation, Quaternion.Euler(_standardCamera),
                Time.deltaTime * 10f);

            _mainCamera.transform.localPosition = Vector3.Lerp(
                _mainCamera.transform.localPosition, _mainCameraOffsets.cameraAbove,
                Time.deltaTime * 5f);
        }
        else
        {
            // _pivot.position = Vector3.Lerp(_pivot.position, _mainCameraOffsets.pivoterPerspective,Time.deltaTime);
                _pitch.transform.rotation =
                Quaternion.Lerp(_pitch.transform.rotation, Quaternion.Euler(_pitcher),
                    Time.deltaTime * 10f);

            _mainCamera.transform.localPosition = Vector3.Lerp(
                _mainCamera.transform.localPosition,
                _mainCameraOffsets.cameraOffset, Time.deltaTime * 5f);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        CameraPanning(data.delta, _mainCameraOffsets);
    }

    private void CameraPanning(Vector2 dragData, CameraOffsets cameraOffsets)
    {
        var position = transform.position;
        _zoomPercentPan = (cameraOffsets.zoomMax - position.y) / (cameraOffsets.zoomMax - cameraOffsets.zoomMin);
        const float floatTolerance = 0.001f;
        if (Math.Abs(cameraOffsets.zoomMax - position.y) < floatTolerance)
        {
            _zoomPercentInverse = 0;
        }
        else
        {
            _zoomPercentInverse =
                (cameraOffsets.zoomMax - cameraOffsets.zoomMin) / (cameraOffsets.zoomMax - position.y);
        }

        /*
        _pivotLerpSpeed = 15.0f; //changed from 25.0f
        var pivoterX = dragData.x * DragRateTop * _zoomPercentInverse * _dpiCompensate;

        _mainCameraOffsets.pivoter.x -= pivoterX;
        _mainCameraOffsets.pivoter.x = Mathf.Clamp(_mainCameraOffsets.pivoter.x,
            _mainCameraOffsets.pivoterBase.x - (_mainCameraOffsets.panRange * _zoomPercentPan),
            _mainCameraOffsets.pivoterBase.x + (_mainCameraOffsets.panRange * _zoomPercentPan));
        _mainCameraOffsets.pivoter.z -= dragData.y * DragRateTop * _zoomPercentInverse * _dpiCompensate;
        _mainCameraOffsets.pivoter.z = Mathf.Clamp(_mainCameraOffsets.pivoter.z,
            _mainCameraOffsets.pivoterBase.z - (_mainCameraOffsets.panRange * _zoomPercentPan),
            _mainCameraOffsets.pivoterBase.z + (_mainCameraOffsets.panRange * _zoomPercentPan));*/
    }

    private void CenterAsZoom(float zoomData, Vector3 zoomPosition, CameraOffsets cameraOffsets)
    {
        if (!scrollZoom) return;
        _mainCameraOffsets.cameraAbove.y -= zoomData * 2.0f;
        _mainCameraOffsets.cameraAbove.y = Mathf.Clamp((float) _mainCameraOffsets.cameraAbove.y,
            cameraOffsets.zoomMin, cameraOffsets.zoomMax);
        _zoomPercentPan = (cameraOffsets.zoomMax - transform.position.y) * (1 / cameraOffsets.zoomMin);
        if (zoomData < 0.0f)
        {
            //Debug.Log("zP < 0"); //ZoomOut
            /*_pivotLerpSpeed = 10.0f;
            _mainCameraOffsets.pivoter.x -= (_mainCameraOffsets.pivoterBase.x - _mainCameraOffsets.pivoter.x) *
                                            _zoomPercentPan * 0.1f;
            _mainCameraOffsets.pivoter.x = Mathf.Clamp(_mainCameraOffsets.pivoter.x,
                _mainCameraOffsets.pivoterBase.x - (_mainCameraOffsets.panRange * _zoomPercentPan),
                _mainCameraOffsets.pivoterBase.x + (_mainCameraOffsets.panRange * _zoomPercentPan));
            _mainCameraOffsets.pivoter.z -= (_mainCameraOffsets.pivoterBase.z - _mainCameraOffsets.pivoter.z) *
                                            _zoomPercentPan * 0.1f;
            _mainCameraOffsets.pivoter.z = Mathf.Clamp(_mainCameraOffsets.pivoter.z,
                _mainCameraOffsets.pivoterBase.z - (_mainCameraOffsets.panRange * _zoomPercentPan),
                _mainCameraOffsets.pivoterBase.z + (_mainCameraOffsets.panRange * _zoomPercentPan));*/
        }

        if (!(zoomData > 0.0f)) return;
        //Debug.Log("zP > 0"); //ZoomIn
        /*_pivotLerpSpeed = 10.0f;
        _mainCameraOffsets.pivoter.x += (zoomPosition.x - (Screen.width * 0.5f)) / Screen.width *
                                        _mainCamera.aspect * CazRate;
        _mainCameraOffsets.pivoter.x = Mathf.Clamp((float) _mainCameraOffsets.pivoter.x,
            _mainCameraOffsets.pivoterBase.x - _mainCameraOffsets.panRange,
            _mainCameraOffsets.pivoterBase.x + _mainCameraOffsets.panRange);
        _mainCameraOffsets.pivoter.z += (zoomPosition.y - (Screen.height * 0.5f)) / Screen.height *
                                        CazRate;
        _mainCameraOffsets.pivoter.z = Mathf.Clamp((float) _mainCameraOffsets.pivoter.z,
            _mainCameraOffsets.pivoterBase.z - _mainCameraOffsets.panRange,
            _mainCameraOffsets.pivoterBase.z + _mainCameraOffsets.panRange);*/
    }

#if UNITY_ANDROID
    private void PinchZoom()
    {
        if (Input.touchCount != 2) return;

        var touchZero = Input.GetTouch(0);
        var touchOne = Input.GetTouch(1);
        _pinchPositionDelta = (touchZero.deltaPosition + touchOne.deltaPosition) / 2.0f;

        _pinchPosition = (touchZero.position + touchOne.position) / 2.0f;
        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
        _deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
        _pinchZoom = _deltaMagnitudeDiff * PinchRate;

        if (Mathf.Abs(_pinchZoom) < PinchIgnore)
        {
            CameraPanning(_pinchPositionDelta, _mainCameraOffsets);
        }
        else if (Mathf.Abs(_pinchZoom) > PinchIgnore)
        {
            CenterAsZoom(_pinchZoom, _pinchPosition, _mainCameraOffsets);
        }
    }
#endif

    #region InputTest

    /*
    public int boardSizeTest = 0;
    public GameObject testCube;
    float startScale = 4.0f;
    public GameObject CameraPivotPrefab;

    public Text boardSizeText;
    public void QuadGameButton()
    {
        switch(boardSizeTest)
        {
            case 0:
                boardSizeTest = 1;
                quadGame = true;
            break;
            case 1:
                boardSizeTest = 2;
            break;
            case 2:
                boardSizeTest = 0;
                quadGame = false;
            break;
            default:
            break;
        }
        AssignCameraOffsets(boardSizeTest);
        boardSizeText.text = boardSizeTest.ToString();
        CameraPivotPrefab.transform.position = mainCameraOffsets.pivoter;
        testCube.transform.localScale = new Vector3((boardSizeTest + 1) * startScale, 1, (boardSizeTest + 1) * startScale);
        testCube.transform.position = mainCameraOffsets.pivoter;
    }

    void OnGUI ()
    {
        GUI.Box(new Rect(10,10,140,20), Screen.orientation.ToString());
        GUI.Box(new Rect(10,40,140,20), "pnchIgn: " + pinchIgnore);
        GUI.Box(new Rect(10, 70, 140, 20), "dltMagDiff: " + deltaMagnitudeDiff);
        GUI.Box(new Rect(10,100,140,20), "pnchPosDlta: " + pinchPositionDelta);
        GUI.Box(new Rect(10,130,140,20), "zoomPerPa: " + zoomPercentPan);
        GUI.Box(new Rect(10,160,140,20), "zoomPerPe: " + zoomPercentPers);
        GUI.Box(new Rect(10, 190, 140, 20), "pvt: " + pivot.position);
        Vector3 pitcherCam = TopCamera ? StandardCamera : pitcher;
        GUI.Box(new Rect(10, 220, 140, 20), "ptchr: " + pitcherCam);
        GUI.Box(new Rect(10, 250, 140, 20), "quadGame: " + quadGame);
    }
    
    public void DragRateTop(string value)
    {
        float.TryParse(value, out dragRateTop);
    }
    
    public void PinchRate(string value)
    {
        float.TryParse(value, out pinchRate);
    }

    public void PinchIgnore(string value)
    {
        float.TryParse(value, out pinchIgnore);
    }
    */

    #endregion
}