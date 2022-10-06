using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NarupaImd;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace ITMO.Scripts
{
    public class Server : MonoBehaviour
    {
        public static readonly UnityEvent LoadingEvent = new UnityEvent();
        public static readonly UnityEvent ConnectedEvent = new UnityEvent();
        public static readonly UnityEvent DisconnectedEvent = new UnityEvent();
        public static readonly UnityEvent FailEvent = new UnityEvent();

        [SerializeField] private NarupaImdSimulation simulation;

        private Process _serverProcess;

        public static bool ServerConnected { get; private set; }
        public static bool ServerLoading { get; private set; }

        private void Awake()
        {
            ConnectedEvent.AddListener(() =>
            {
                ServerConnected = true;
                ServerLoading = false;
            });
            LoadingEvent.AddListener(() => ServerLoading = true);
            FailEvent.AddListener(() => ServerLoading = false);
        }

        public void Start()
        {
            if (!TestTcpConnection()) RunServerProcess();
        }

        public void OnApplicationQuit()
        {
            Disconnect();
            if (_serverProcess == null) return;
            var client = new TcpClient("localhost", 7777);
            var buffer = Encoding.UTF8.GetBytes("q");
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.Close();
            Thread.Sleep(1000);
            try
            {
                _serverProcess.StandardInput.Write("");
                _serverProcess.StandardInput.Flush();
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void RunServerProcess()
        {
            try
            {
                _serverProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = Application.dataPath + "\\..\\run_server.bat",
                        WorkingDirectory = Application.dataPath + "\\..\\",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        CreateNoWindow = true
                    }
                };


                _serverProcess.Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static bool TestTcpConnection(string host = "127.0.0.1", int port = 7777,
            int timeout = 3000)
        {
            var client = new TcpClient();
            try
            {
                var result = client.BeginConnect(host, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout);
                if (success) client.EndConnect(result);
                client.Close();
                return success;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public async void Connect()
        {
            LoadingEvent.Invoke();
            if (ServerConnected) return;
            for (var i = 0; i < 5; i++)
            {
                await simulation.AutoConnect(millisecondsTimeout: 1000);
                if (simulation.gameObject.activeSelf) break;
            }

            if (!simulation.gameObject.activeSelf)
            {
                FailEvent.Invoke();
                throw new Exception("Cannot connect to Narupa server");
            }

            for (var i = 0; i < 10; i++)
            {
                Debug.Log(i);
                if (EyeInteraction.ParticlesCount == 0) await Task.Delay(1000);
                else break;
            }

            if (EyeInteraction.ParticlesCount == 0)
            {
                FailEvent.Invoke();
                throw new Exception("Cannot connect to Narupa server");
            }
            
            ConnectedEvent.Invoke();
        }

        public static async void Send(string pathToMolecule, string host = "127.0.0.1", int port = 7777)
        {
            try
            {
                var client = new TcpClient(host, port);
                var buffer = Encoding.UTF8.GetBytes(pathToMolecule);
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // private Task<string> Read()
        // {
        //     throw new NotImplementedException();
        //     // var buffer = new byte[256];
        //     // await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
        //     // return Encoding.UTF8.GetString(buffer);
        // }

        public void Disconnect()
        {
            if (!ServerConnected) return;
            simulation.Disconnect();
            ServerConnected = false;
            DisconnectedEvent.Invoke();
        }
    }
}