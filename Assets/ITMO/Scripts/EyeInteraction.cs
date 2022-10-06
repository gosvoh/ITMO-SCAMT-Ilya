using System;
using System.Collections.Generic;
using System.Diagnostics;
using Narupa.Frame;
using Narupa.Visualisation;
using NarupaImd.Interaction;
using UnityEngine;
using UnityEngine.Events;
using ViveSR.anipal.Eye;

namespace ITMO.Scripts
{
    public class EyeInteraction : MonoBehaviour
    {
        [SerializeField] private SynchronisedFrameSource frameSource;
        [SerializeField] private GameObject atomPrefab;
        [SerializeField] private GameObject wallsParent;
        [SerializeField] private GameObject simulationSpace;
        [SerializeField] private InteractableScene interactableScene;

        public static Logger Logger;
        public static int EyeGazeChangedCounter;
        public static int FrameBreakpoint = 10;
        public static int LastID = -10;
        public static int ParticlesCount;

        private static readonly Dictionary<int, Info> Spheres = new Dictionary<int, Info>();

        private GameObject _parent;
        private int _counter = -1;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private void Awake()
        {
            SetWalls();
            Server.ConnectedEvent.AddListener(ConnectionHandler);
            Server.DisconnectedEvent.AddListener(EventHandler);
            PlayerPrefs.GetInt("EyeInteraction.FrameBreakpoint", 10);
        }

        private void ConnectionHandler()
        {
            Logger = new Logger();
            Logger.AddInfo("SKD|timestamp|position|ID");
            EyeGazeChangedCounter = 0;
            _stopwatch.Restart();
        }

        private void EventHandler()
        {
            _stopwatch.Stop();
            Logger.AddInfo(
                $"Level - {Level.CurrentLevelName}; Time spent in seconds - {_stopwatch.Elapsed.TotalSeconds}; Gaze changed - {EyeGazeChangedCounter}");
            Logger.WriteInfo();
            EyeGazeChangedCounter = 0;
            frameSource.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Destroy(_parent);
            Spheres.Clear();
        }

        private void FixedUpdate()
        {
            if (frameSource.CurrentFrame is null) return;
            if (_counter++ % FrameBreakpoint != 0) return;
            UpdateScene();
            CheckGaze();
        }

        private void CheckGaze()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;
            var eyeFocus = SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out _, out var focusInfo, 0,
                float.MaxValue, 1 << atomPrefab.layer);
            if (!eyeFocus) return;
            var info = focusInfo.transform.GetComponent<Info>();
            if (info is null || info.Index == LastID) return;
            LastID = info.Index;
            EyeGazeChangedCounter++;
            if (Logger == null) return;
            Logger.AddInfo(
                $"SRanipal|{DateTime.Now:HH:mm:ss.fff}|{info.Obj.transform.position.ToString("F6")}|{info.Index}");
            Logger.WriteInfo();
        }

        private void UpdateScene()
        {
            var frame = frameSource.CurrentFrame;
            if (frame.ParticleCount == 0) return;
            if (Spheres.Count == frame.ParticleCount)
            {
                var particlePositions = frame.ParticlePositions;
                for (var i = 0; i < particlePositions.Length; i++)
                    Spheres[i].gameObject.transform.localPosition = particlePositions[i];
            }
            else
            {
                Destroy(_parent);
                Spheres.Clear();
                CreateSphere(frame);
            }

            ParticlesCount = frame.ParticleCount;
        }

        private void CreateSphere(Frame frame)
        {
            // Reset simulation space
            simulationSpace.transform.localPosition = Vector3.zero;
            simulationSpace.transform.rotation = new Quaternion();

            _parent = new GameObject("ParentEyeInteraction")
            {
                transform =
                {
                    parent = interactableScene.transform,
                    localPosition = Vector3.zero,
                    localScale = Vector3.one,
                    rotation = new Quaternion()
                }
            };

            var particles = frame.Particles;
            var particlePositions = frame.ParticlePositions;
            for (int i = 0, partCount = frame.ParticleCount; i < partCount; ++i)
            {
                var atom = Instantiate(atomPrefab, particlePositions[i], Quaternion.identity);
                atom.transform.SetParent(_parent.transform, false);
                var info = atom.GetComponent<Info>();
                info.Index = particles[i].Index;
                info.Obj = atom;
                Spheres.Add(info.Index, info);
            }
        }

        private void SetWalls()
        {
            for (var i = 0; i < wallsParent.transform.childCount; i++)
            {
                var wall = wallsParent.transform.GetChild(i).gameObject;
                var wallInfo = wall.GetComponent<Info>();
                wallInfo.Index = -(i + 1);
                wallInfo.Obj = wall;
            }
        }
    }
}