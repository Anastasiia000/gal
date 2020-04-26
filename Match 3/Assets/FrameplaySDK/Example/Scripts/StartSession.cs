using UnityEngine;
using UnityEngine.Analytics;

namespace FrameplaySDK.Example
{
    public class StartSession : MonoBehaviour
    {
        public FrameplayDataAsset DataAsset;
        public Camera Camera;

        void Start()
        {
            if (!Frameplay.SessionStarted)
            {
                Frameplay.StartSession(DataAsset, -1, Gender.Unknown, "", SystemLanguage.Unknown, RegisterCamera);
            }
            else
            {
                Frameplay.RegisterCamera(Camera);
            }
        }

        private void RegisterCamera()
        {
            Frameplay.RegisterCamera(Camera);
        }
    }
}

