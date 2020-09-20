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
        private Thread _socketThread = null;
        private string _nudgeMsg = "";

        public void Connect()
        {
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
            _nudgeMsg = dir.ToString();
            Debug.Log("Nudging: " + _nudgeMsg);
        }

        private void MessageReceived(string message)
        {
            var data = message.Split(',');
            var unitytime = DateTime.Now.ToString("HH:mm:ss.ffffff");
            
            if (float.TryParse(data[0], out _elbowValue)){
                OnElbowValue?.Invoke(new UdpEntry(ELBOW_ID, _elbowValue, data[2], unitytime));
            }

            if (float.TryParse(data[1], out _wristValue))
            {
                OnWristValue?.Invoke(new UdpEntry(WRIST_ID, _wristValue, data[2], unitytime));
            }
        }

        private void ExecuteServer()
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _serverPort);

            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            newsock.Bind(ipep);
            Debug.Log("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);

            recv = newsock.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from {0}:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            //string welcome = "Welcome to my test server";
            data = Encoding.UTF8.GetBytes($"\"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)}\"");
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                data = new byte[1024];
                recv = newsock.ReceiveFrom(data, ref Remote);

                MessageReceived(Encoding.ASCII.GetString(data, 0, recv));

                data = Encoding.ASCII.GetBytes(_nudgeMsg);
                newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
                _nudgeMsg = ""; //Clear the nudgemsg after it's sent.
            }
        }

        void OnApplicationQuit()
        {
            Close();

        }
    }
}
