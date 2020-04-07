using UnityEngine;

namespace MyScriptableObjects
{
	[CreateAssetMenu(fileName = "CameraSettings", menuName = "MyAssets/Camera Settings", order = 55)]
	public class CameraSettings : ScriptableObject
	{
		public Vector3 cameraOffset;
		public Vector3 cameraAbove;
		public float zoomMax;
		public float zoomMin;
		// public Vector3 pivoterBase;
		public float panRange;
		public float pivotLerpSpeed;
		// public float panLerpSpeed;
		public float zoomLerpSpeed;
		public float zoomMultiplier;
		public float panMultiplier;
		
		public float dragRateTop;
		// public float dpiCompensate;
		public float cazRate;
		public float pinchRate;
		public float pinchIgnore;
	}
}