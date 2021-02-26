using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Common
{
    public class FrameRateLimiter : MonoBehaviour
    {
        public int vSyncCount = 0;
        public int targetFrameRate = 200;
        // Start is called before the first frame update
        void Start()
        {
            QualitySettings.vSyncCount = vSyncCount;
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
