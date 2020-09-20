using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Udp
{
    public class UdpHost : MonoBehaviour
    {
        [SerializeField] private Int32 _port = 5013;
        [SerializeField] private string _ip = "127.0.0.1";
        private Thread _socketThread = null;

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

        public void Nudge(NudgeDir dir)
        {

        }

        private void ExecuteServer()
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _port);

            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            newsock.Bind(ipep);
            Debug.Log("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);

            recv = newsock.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from {0}:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            //Todo: send the time instead?
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                data = new byte[1024];
                recv = newsock.ReceiveFrom(data, ref Remote);

                Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
                newsock.SendTo(data, recv, SocketFlags.None, Remote);
            }
        }

        void OnApplicationQuit()
        {
            Close();

        }
    }
}
