using System;
using TMPro;
using Tobii.G2OM;
using UnityEngine;
using UnityEngine.Serialization;

namespace ITMO.Scripts
{
    public class Info : MonoBehaviour, IGazeFocusable
    {
        [SerializeField]
        private TMP_Text textObj;
        [SerializeField]
        private Canvas canvas;
        public int Index { private set; get; }
        public GameObject Obj { set; get; }

        public enum Type
        {
            WALL,
            ATOM
        }

        public Type IsAtom = Type.ATOM;

        [FormerlySerializedAs("Material")] [HideInInspector] public Material material;
        private Renderer _renderer;
        private Camera _camera;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            material = _renderer.material;
            _camera = Camera.main;
        }

        public void SetIndex(int index)
        {
            Index = index;
            if (textObj) textObj.text = index.ToString();
        }

        private void FixedUpdate()
        {
            if (IsAtom != Type.ATOM) return;
            _renderer.enabled = EyeInteraction.EnableHighlight;
            var transparent = material.color;
            transparent.a = 0;
            if (!EyeInteraction.EnableHighlight) material.color = transparent;
            textObj.enabled = EyeInteraction.EnableAtomIds;
            if (!_camera) return;
            textObj.transform.LookAt(_camera.transform);
            canvas.transform.rotation = Quaternion.LookRotation(_camera.transform.forward);
        }

        public void GazeFocusChanged(bool hasFocus)
        {
            if (!hasFocus) return;
            if (EyeInteraction.ParticlesCount == 0 || EyeInteraction.Logger == null) return;
            EyeInteraction.LastInfo = this;
            EyeInteraction.EyeGazeChangedCounter++;
            if (EyeInteraction.Logger == null) return;
            EyeInteraction.Logger.AddInfo(
                $"Tobii|{DateTime.Now:HH:mm:ss.fff}|{Obj.transform.position.ToString("F6")}|{Index}");
            EyeInteraction.Logger.WriteInfo();
        }
    }
}