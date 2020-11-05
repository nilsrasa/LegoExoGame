using System;
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
        public static event Action<string> OnReceiveMsg;

        [Header("Host settings")]
        [SerializeField] protected Int32 _hostPort = 5013;
        [SerializeField] protected string _hostIp = "127.0.0.1";
        [Header("Client settings")]
        [SerializeField] protected Int32 _clientPort = 5011;
        [SerializeField] protected string _clientIp = "127.0.0.1";
        [SerializeField, Tooltip("Set true if host should auto start and connect to the client")]
        protected bool _autoConnect = false;
        [Header("Stream")]
        [SerializeField] protected string _message;
        protected Thread _socketThread = null;
        protected bool _connected;
        protected EndPoint _client;
        protected Socket _socket;

        public virtual void Start()
        {
           if (_autoConnect) Connect();
        }

        /// <summary>
        /// Opens a connection to the set client
        /// </summary>
        public virtual void Connect()
        {
            _socketThread = new Thread(ExecuteHost);
            _socketThread.IsBackground = true;
            _socketThread.Start();
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public virtual void Close()
        {
            _connected = false;
            _socket.Close();
            _socketThread.Interrupt();
            _socketThread.Join();
        }

        #region Settings
        public virtual void SetClient(string clientIp, Int32 clientPort)
        {
            //TODO: check if Ip is valid?
            _clientIp = clientIp;
            _clientPort = clientPort;
        }

        public virtual void SetHost(string hostIp, Int32 hostPort)
        {
            _hostIp = hostIp;
            _hostPort = hostPort;
        }

        #endregion

        #region Comms
        /// <summary>
        /// Send a message to the client
        /// </summary>
        /// <param name="msg"></param>
        public virtual void SendMsg(string msg)
        {
            if (_connected)
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                _socket.SendTo(data, data.Length, SocketFlags.None, _client);
            }
        }

        /// <summary>
        /// Called when a message is received
        /// </summary>
        /// <param name="message">string msg</param>
        public virtual void MessageReceived(string message)
        {
            _message = message;

            OnReceiveMsg?.Invoke(message);
        }

        /// <summary>
        /// Starts the host and runs a loop waiting for messages.
        /// </summary>
        protected void ExecuteHost()
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(_hostIp), _hostPort);

            _socket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            _socket.Bind(ipep);
            Debug.Log("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_clientIp), _clientPort);
            _client = (EndPoint)(sender);

            //string welcome = "Welcome to my test host";
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
        #endregion

        public virtual void OnApplicationQuit()
        {
            //Close();

        }
    }
}
