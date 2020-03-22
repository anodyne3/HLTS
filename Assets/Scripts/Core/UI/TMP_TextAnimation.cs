using System;
using MyScriptableObjects;
using TMPro;

namespace Core.UI
{
    [Serializable]
    public struct TMP_TextAnimation
    {
        public TMP_VertexAnimation vertexAnimation;
        public TMP_Text text;

        public void Init()
        {
            vertexAnimation.Init(text);
        }
    }
}