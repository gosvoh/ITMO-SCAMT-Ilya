using System;
using System.Linq;
using Castle.Core.Internal;
using Narupa.Visualisation;
using NarupaImd.UI;
using Tobii.XR.GazeModifier;
using UnityEngine;
using ViveSR.anipal.Eye;

namespace ITMO.Scripts
{
    public class DesktopUI : MonoBehaviour
    {
        [SerializeField] private GameObject app;
        [SerializeField] private UserInterfaceManager manager;
        [SerializeField] private GameObject scene;
        [SerializeField] private BoxVisualiser box;
        [SerializeField] private GameObject walls;
        [SerializeField] private GazeModifierSettings gazeModifierSettings;

        private const string ChooseMsg = "Choose level";
        private const string EmptyMsg = "Empty list of levels";

        private string _levelToShow = ChooseMsg;
        private string[] _list;
        private Vector2 _scrollViewVector = Vector2.zero;
        private bool _showDropdown;
        private Server _server;
        private App _app;

        private void Awake()
        {
            _app = app.GetComponent<App>();
            _server = app.GetComponent<Server>();
            Level.Initialize();
            _list = Level.LevelNamesList.IsNullOrEmpty() ? Array.Empty<string>() : Level.LevelNamesList.ToArray();

            if (PlayerPrefs.HasKey("EnableBox"))
                box.gameObject.SetActive(bool.Parse(PlayerPrefs.GetString("EnableBox")));
            if (PlayerPrefs.HasKey("TrackWalls"))
                walls.SetActive(bool.Parse(PlayerPrefs.GetString("TrackWalls")));
            if (PlayerPrefs.HasKey("TobiiEnabled"))
                EyeTrackerSwitcher.TobiiEnabled = bool.Parse(PlayerPrefs.GetString("TobiiEnabled"));
            if (PlayerPrefs.HasKey("FrameBreakpoint"))
                EyeInteraction.FrameBreakpoint = PlayerPrefs.GetInt("FrameBreakpoint");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16, 16, 192, 512));
            if (!Server.ServerConnected)
            {
                if (Server.ServerLoading)
                {
                    GUILayout.Box($"Loading {Level.CurrentLevelName}...");
                    GUILayout.EndArea();
                    return;
                }

                if (GUILayout.Button("Send and connect"))
                {
                    if (_levelToShow.Equals(ChooseMsg))
                    {
                        Level.CurrentLevelNode = Level.LevelNamesList.First;
                        Level.CurrentLevelName = Level.CurrentLevelNode.Value;
                        Server.Send(Level.GetLevelPath(Level.CurrentLevelName));
                    }
                    else
                    {
                        Server.Send(Level.GetLevelPath(_levelToShow));
                        Level.CurrentLevelName = _levelToShow;
                        Level.CurrentLevelNode = Level.LevelNamesList.Find(_levelToShow);
                    }

                    _server.Connect();
                }

                if (GUILayout.Button("Connect")) _server.Connect();
                if (GUILayout.Button("Exit")) _app.Quit();
                if (GUILayout.Button(_levelToShow)) _showDropdown = !_showDropdown;

                if (_showDropdown)
                {
                    _scrollViewVector = GUILayout.BeginScrollView(_scrollViewVector, GUILayout.MaxHeight(200));
                    if (_list.Length == 0) GUILayout.Box(EmptyMsg);

                    foreach (var s in _list)
                    {
                        if (!GUILayout.Button(s)) continue;
                        _showDropdown = false;
                        _levelToShow = s;
                    }

                    GUILayout.EndScrollView();
                }

                if (GUILayout.Button("Launch eye calibration")) SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
            }
            else
            {
                if (Level.CurrentLevelName != null)
                {
                    GUILayout.Box(Level.CurrentLevelName);
                    if (GUILayout.Button(Level.CurrentLevelNode?.Previous == null
                            ? "Disconnect"
                            : "Back to " + Level.CurrentLevelNode.Previous.Value))
                    {
                        if (Level.CurrentLevelNode?.Previous != null)
                        {
                            Level.CurrentLevelNode = Level.CurrentLevelNode.Previous;
                            Level.CurrentLevelName = Level.CurrentLevelNode.Value;
                            _server.Disconnect();
                            Server.Send(Level.GetLevelPath(Level.CurrentLevelName));
                            _server.Connect();
                        }
                        else DisconnectAndReturn();
                    }

                    if (GUILayout.Button(Level.CurrentLevelNode?.Next == null
                            ? "Disconnect"
                            : "Next to " + Level.CurrentLevelNode.Next.Value))
                    {
                        if (Level.CurrentLevelNode?.Next != null)
                        {
                            Level.CurrentLevelNode = Level.CurrentLevelNode.Next;
                            Level.CurrentLevelName = Level.CurrentLevelNode.Value;
                            _server.Disconnect();
                            Server.Send(Level.GetLevelPath(Level.CurrentLevelName));
                            _server.Connect();
                        }
                        else DisconnectAndReturn();
                    }
                }

                if (GUILayout.Button("Disconnect")) DisconnectAndReturn();
            }


            var tobiiState = GUILayout.Toggle(EyeTrackerSwitcher.TobiiEnabled, "Enable Tobii");
            if (tobiiState != EyeTrackerSwitcher.TobiiEnabled)
            {
                EyeTrackerSwitcher.TobiiEnabled = tobiiState;
                PlayerPrefs.SetString("TobiiEnabled", tobiiState.ToString());
                if (tobiiState) EyeTrackerSwitcher.EnableTobii();
                else EyeTrackerSwitcher.DisableTobii();
            }

            if (EyeTrackerSwitcher.TobiiEnabled)
            {
                GUILayout.Box($"Tobii quality: {EyeTrackerSwitcher.TobiiQuality}");
                var quality = (int)GUILayout.HorizontalSlider(EyeTrackerSwitcher.TobiiQuality, 0, 100);
                if (quality != EyeTrackerSwitcher.TobiiQuality)
                {
                    EyeTrackerSwitcher.TobiiQuality = quality;
                    gazeModifierSettings.SelectedPercentileIndex = EyeTrackerSwitcher.TobiiQuality;
                    PlayerPrefs.SetInt("EyeTrackerSwitcher.TobiiQuality", EyeTrackerSwitcher.TobiiQuality);
                }
            }

            if (!EyeTrackerSwitcher.TobiiEnabled)
            {
                GUILayout.Box($"Update positions every {EyeInteraction.FrameBreakpoint * Time.fixedDeltaTime:0.00}s");
                var frameBreakpoint = (int)GUILayout.HorizontalSlider(EyeInteraction.FrameBreakpoint, 1, 50);
                if (frameBreakpoint != EyeInteraction.FrameBreakpoint)
                {
                    EyeInteraction.FrameBreakpoint = frameBreakpoint;
                    PlayerPrefs.SetInt("FrameBreakpoint", frameBreakpoint);
                }
            }

            var enabledHighlight = GUILayout.Toggle(EyeInteraction.EnableHighlight, "Enable atom highlight");
            if (enabledHighlight != EyeInteraction.EnableHighlight)
            {
                EyeInteraction.EnableHighlight = enabledHighlight;
                PlayerPrefs.SetString("EyeInteraction.EnableHighlight", enabledHighlight.ToString());
            }

            var enableAtomIds = GUILayout.Toggle(EyeInteraction.EnableAtomIds, "Enable atom ids");
            if (enableAtomIds != EyeInteraction.EnableAtomIds)
            {
                EyeInteraction.EnableAtomIds = enableAtomIds;
                PlayerPrefs.SetString("EyeInteraction.EnableAtomIds", enableAtomIds.ToString());
            }

            GUILayout.EndArea();
        }

        private void DisconnectAndReturn()
        {
            _levelToShow = ChooseMsg;
            manager.GotoScene(scene);
            _app.Disconnect();
        }
    }
}