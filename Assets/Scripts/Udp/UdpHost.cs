using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Udp
{
    public class UdpHost : MonoBehaviour
    {
        public static System.Action<UdpEntry> OnElbowValue, OnWristValue;

        [SerializeField] private Int32 _serverPort = 5013;
        [SerializeField] private string _ip = "127.0.0.1";
        [SerializeField] private float _elbowValue, _wristValue;
        private const string ELBOW_ID = "elbow", WRIST_ID = "wrist";
        private Int32 _clientPort;
        private string _clientIp;
        private Thread _socketThread = null;
        private bool _connected;
        private EndPoint _client;
        private Socket _socket;

        public void Connect(string ip, Int32 port)
        {
            _clientIp = ip;
            _clientPort = port;
            _socketThread = new Thread(ExecuteServer);
            _socketThread.IsBackground = true;
            _socketThread.Start();
        }

        public void Close()
        {
            _socketThread.Abort();
        }


        public void Nudge(int i)
        {
            Nudge((NudgeDir)i);
        }
        public void Nudge(NudgeDir dir)
        {
            if (_connected)
            {
                string msg = dir.ToString();

                byte[] data = Encoding.ASCII.GetBytes(msg);
                _socket.SendTo(data, data.Length, SocketFlags.None, _client);
            }
        }

        private void MessageReceived(string message)
        {
            var data = message.Split(',');
            var unitytime = DateTime.Now.ToString("HH:mm:ss.ffffff");
            
            if (data.Length == 3)
            {
                if (float.TryParse(data[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _elbowValue))
                {
                    OnElbowValue?.Invoke(new UdpEntry(ELBOW_ID, _elbowValue, data[2], unitytime));
                }

                if (float.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _wristValue))
                {
                    OnWristValue?.Invoke(new UdpEntry(WRIST_ID, _wristValue, data[2], unitytime));
                }
            }
        }

        private void ExecuteServer()
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _serverPort);

            _socket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            _socket.Bind(ipep);
            Debug.Log("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_clientIp), _clientPort);
            _client = (EndPoint)(sender);

            //string welcome = "Welcome to my test server";
            data = Encoding.UTF8.GetBytes($"\"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)}\"");
            _socket.SendTo(data, data.Length, SocketFlags.None, _client);

            //TODO: catch error if client is nonexistent and handle it!!!

            _connected = true;

            while (true)
            {
                data = new byte[1024];
                recv = _socket.ReceiveFrom(data, ref _client);

                MessageReceived(Encoding.ASCII.GetString(data, 0, recv));
            }
        }

        void OnApplicationQuit()
        {
            //Close();

        }
    }
}
