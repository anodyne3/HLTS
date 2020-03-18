using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "MusicTrack - ", menuName = "MyAssets/MusicTrack", order = 30)]
    public class MusicTrack : ScriptableObject
    {
        public AudioClip musicTracks;
    }
}