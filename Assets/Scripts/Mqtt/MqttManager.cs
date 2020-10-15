using Game;
using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt
{
    public class MqttManager : GameMqttClient
    {
        public string elbowValue, wristValue;

        [SerializeField] private string _clientIp = "192.168.0.101";
        [SerializeField] private string _dateTimeTopic = "time";
        [SerializeField] private string _nudgeTopic = "nudge";
        [SerializeField] private string _elbowTopic = "motor_value_elbow";
        [SerializeField] private string _wristTopic = "motor_value_wrist";
        [SerializeField] private string _elbowCommand = "motor_command_elbow";
        [SerializeField] private string _wristCommand = "motor_command_wrist";

        private MqttClient _client;
        public bool isConnected { get; private set; }

        public override void Connect(string clientIp)
        {
            _client = new MqttClient(clientIp);
            byte code = _client.Connect("unity_program");

            //Subscribe to the mqtt events
            _client.MqttMsgPublished += client_MqttMsgPublished;
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //Send the current machine time to sync up with the exo
            ushort publishId = _client.Publish(_dateTimeTopic, Encoding.UTF8.GetBytes($"\"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)}\""));

            //Subscribe to the exo's wrist and elbow messages
            ushort subscribeId = _client.Subscribe(new string[] { _wristTopic, _elbowTopic },
                    new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            isConnected = true;//TODO: should probably check that a connection was established

            
        }

        public override void Close()
        {
            //FIxme: nullreference??
            _client.MqttMsgPublished -= client_MqttMsgPublished;
            _client.MqttMsgPublishReceived -= client_MqttMsgPublishReceived;
            _client.Disconnect();
            _client = null;

            //Todo: reset datetime, stopwatch etc..

            isConnected = false;
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //Debug.Log("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //Debug.Log("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

            if (e.Topic == _elbowTopic)
            {
                // Do anything with the value
                elbowValue = Encoding.UTF8.GetString(e.Message);

                //Pass on the value to all event subscribers
                OnElbowValue?.Invoke(MessageToEntry(_elbowTopic, elbowValue));
            }
            else if (e.Topic == _wristTopic)
            {
                // Do anything with the value
                wristValue = Encoding.UTF8.GetString(e.Message);

                //Pass on the value to all event subscribers
                OnWristValue?.Invoke(MessageToEntry(_wristTopic, wristValue));
            }
        }

        /*public void SetMotorSpeed(int speed, bool isElbow)
        {
            string topic = (isElbow) ? _elbowCommand : _wristCommand;

            PublishMessage(speed.ToString(), topic);
        }*/

        public override void Nudge(NudgeDir dir)
        {
            PublishMessage(_nudgeTopic, dir.ToString());
        }

        private ushort PublishMessage(string topic, string msg)
        {
            return _client.Publish(topic, Encoding.UTF8.GetBytes(msg));
        }

        /// <summary>
        /// Used to translate the recieved mqtt message into an MqqtEntry
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private MqttEntry MessageToEntry(string id, string msg)
        {
            var split = msg.Split(',');
            var value = float.Parse(split[0]);
            var mqtttime = split[1];
            var unitytime = DateTime.Now.ToString("HH:mm:ss.ffffff");

            return new MqttEntry(id, value, mqtttime, unitytime);
        }

        private void OnDestroy()
        {
            Close();
        }
    }
}
