using System;
using Tobii.G2OM;
using UnityEngine;

namespace ITMO.Scripts
{
    public class Info : MonoBehaviour, IGazeFocusable
    {
        public int Index { set; get; }
        public GameObject Obj { set; get; }

        public void GazeFocusChanged(bool hasFocus)
        {
            if (!hasFocus) return;
            if (EyeInteraction.ParticlesCount == 0 || EyeInteraction.Logger == null) return;
            EyeInteraction.LastID = Index;
            EyeInteraction.EyeGazeChangedCounter++;
            if (EyeInteraction.Logger == null) return;
            EyeInteraction.Logger.AddInfo($"Tobii|{DateTime.Now:HH:mm:ss.fff}|{Obj.transform.position.ToString("F6")}|{Index}");
            EyeInteraction.Logger.WriteInfo();
        }
    }
}