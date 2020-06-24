using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt
{
    public class MqttManager : MonoBehaviour
    {
        public string elbowValue, wristValue;

        [SerializeField] private string _clientIp = "192.168.0.101";
        [SerializeField] private string _elbowTopic = "motor_value_elbow";
        [SerializeField] private string _wristTopic = "motor_value_wrist";
        [SerializeField] private string _elbowCommand = "motor_command_elbow";
        [SerializeField] private string _wristCommand = "motor_command_wrist";

        private MqttClient _client;
        public bool isConnected { get; private set; }
        
        public event System.Action<MqttEntry> OnElbowValue, OnWristValue;

        public void Connect()
        {
            _client = new MqttClient(_clientIp);
            byte code = _client.Connect("unity_program");

            _client.MqttMsgPublished += client_MqttMsgPublished;
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //ushort publishId = _client.Publish("unity_topic",Encoding.UTF8.GetBytes("test_message"));

            ushort subscribeId = _client.Subscribe(new string[] { _wristTopic, _elbowTopic },
                    new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            isConnected = true;//TODO: should probably check that a connection was established
        }

        public void Close()
        {
            _client.MqttMsgPublished -= client_MqttMsgPublished;
            _client.MqttMsgPublishReceived -= client_MqttMsgPublishReceived;
            _client.Disconnect();
            _client = null;

            isConnected = false;
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Debug.Log("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.Log("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

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

        public void Nudge(NudgeDir dir)
        {
            //nudge_up.down.left.right
            string topic = "nudge";

            PublishMessage(topic, dir.ToString());
        }

        private ushort PublishMessage(string msg, string topic)
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
            var time = split[1];

            return new MqttEntry(id, value, time);
        }

        private void OnDestroy()
        {
            Close();
        }
    }

    public enum NudgeDir
    {
        up,
        down,
        left,
        right
    }
}
