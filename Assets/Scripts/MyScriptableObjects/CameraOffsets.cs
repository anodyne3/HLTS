using UnityEngine;

namespace MyScriptableObjects
{
	[CreateAssetMenu(fileName = "CameraOffset", menuName = "MyAssets/Camera Offset", order = 55)]
	public class CameraOffsets : ScriptableObject
	{
		public Vector3 cameraOffset;
		public Vector3 cameraAbove;
		public float zoomMax;
		public float zoomMin;
		public Vector3 pivoterBase;
		public float panRange;
		public float pivotLerpSpeed = 10.0f;
		public float panLerpSpeed = 15.0f;
		public float zoomLerpSpeed = 1.0f;
		public float zoomMultiplier = 2.0f;
	}
}