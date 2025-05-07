using System.Collections;
using UnityEngine;

namespace AiGameProject.Utils
{
    public class Timer : MonoBehaviour
    {
        public static Timer Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void Delay(float seconds, System.Action callback)
        {
            StartCoroutine(DelayCoroutine(seconds, callback));
        }

        private IEnumerator DelayCoroutine(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
    }
}
