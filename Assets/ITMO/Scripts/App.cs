using System;
using NarupaImd.UI;
using UnityEngine;

namespace ITMO.Scripts
{
    public class App : MonoBehaviour
    {
        [SerializeField] private Server server;
        [SerializeField] private UserInterfaceManager interfaceManager;
        [SerializeField] private GameObject loadingScreen;

        private void Awake()
        {
            server = GetComponent<Server>();
            Server.LoadingEvent.AddListener(LoadingHandler);
            Server.FailEvent.AddListener(FailHandler);
        }

        private void LoadingHandler() => interfaceManager.GotoSceneAndAddToStack(loadingScreen);

        private void FailHandler() => interfaceManager.GoBack();

        public void Quit() => Application.Quit();

        public void Disconnect()
        {
            server.Disconnect();
            Level.CurrentLevelNode = null;
            Level.CurrentLevelName = null;
        }

        private void OnApplicationQuit() => Quit();
    }
}