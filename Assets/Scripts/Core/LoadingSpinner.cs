using System.Collections;
using UnityEngine;
using Utils;

namespace Core
{
    public class LoadingSpinner : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(nameof(Spin));
        }

        private IEnumerator Spin()
        {
            var waitForEndOfFrame = new WaitForEndOfFrame();
            while (gameObject.activeInHierarchy)
            {
                transform.Rotate(0.0f, 0.0f, -Constants.LoadingSpinnerDegrees);
                yield return waitForEndOfFrame;
            }
        }
    }
}