using Tobii.XR;
using Tobii.XR.GazeModifier;
using UnityEngine;
using ViveSR.anipal.Eye;

namespace ITMO.Scripts
{
    public class EyeTrackerSwitcher : MonoBehaviour
    {
        public static bool TobiiEnabled;
        private bool _tobiiEnabled;
        public static int TobiiQuality;

    private GazeModifierSettings gazeModifierSettings;

        private void Awake()
        {
            _tobiiEnabled = TobiiEnabled;
            TobiiQuality = PlayerPrefs.GetInt("EyeTrackerSwitcher.TobiiQuality", 85);
            gazeModifierSettings = GetComponent<GazeModifierSettings>();
            if (gazeModifierSettings != null) gazeModifierSettings.SelectedPercentileIndex = TobiiQuality;
        }

        // private void FixedUpdate()
        // {
        //     if (TobiiEnabled) EnableTobii();
        //     else DisableTobii();
        // }
        //
        // private void DisableTobii()
        // {
        //     if (!_tobiiEnabled) return;
        //     TobiiXR.Stop();
        //     // SRanipal_Eye_Framework.Instance.EnableEye = true;
        //     // SRanipal_Eye_Framework.Instance.StartFramework();
        //     Debug.LogWarning("Tobii disabled");
        //     _tobiiEnabled = false;
        //     if (PlayerPrefs.HasKey("FrameBreakpoint"))
        //         EyeInteraction.FrameBreakpoint = PlayerPrefs.GetInt("FrameBreakpoint");
        // }
        //
        // private void EnableTobii()
        // {
        //     if (_tobiiEnabled) return;
        //     // SRanipal_Eye_Framework.Instance.EnableEye = false;
        //     // SRanipal_Eye_Framework.Instance.StopFramework();
        //     Debug.LogWarning("Tobii enabled? " + TobiiXR.Start(new TobiiXR_Settings()));
        //     _tobiiEnabled = true;
        // }

        public static void DisableTobii()
        {
            TobiiXR.Stop();
            Debug.Log("Tobii disabled");
            if (PlayerPrefs.HasKey("FrameBreakpoint"))
                EyeInteraction.FrameBreakpoint = PlayerPrefs.GetInt("FrameBreakpoint");
        }
        
        public static void EnableTobii()
        {
            Debug.Log("Tobii enabled? " + TobiiXR.Start(new TobiiXR_Settings()));
            EyeInteraction.FrameBreakpoint = 10;
        }
    }
}