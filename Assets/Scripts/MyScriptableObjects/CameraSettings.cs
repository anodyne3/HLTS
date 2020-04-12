using UnityEngine;

namespace MyScriptableObjects
{
	[CreateAssetMenu(fileName = "CameraSettings", menuName = "MyAssets/Camera Settings", order = 55)]
	public class CameraSettings : ScriptableObject
	{
		public float zoomMax;
		public float zoomMin;
		public float panRange;
		public float panLerpSpeed;
		public float panMultiplier;
		
		public float zoomLerpSpeed;
		public float zoomMultiplier;
		
		public float pinchRate;
		public float pinchIgnore;
	}
}