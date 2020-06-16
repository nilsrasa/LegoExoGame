﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt
{
    public class MqttManager : MonoBehaviour
    {
        public string directionFromMindstorms;
        // Create some MQTT broker subscription

        public float speed = 0.01f;
        public float vertical = 0f;
        public float horizontal = 0f;
        public string elbowValue, wristValue;
        private float _elbowAngle, _wristAngle;

        private string _clientIp = "192.168.0.101";
        private string _elbowTopic = "motor_value_elbow";
        private string _wristTopic = "motor_value_wrist";

        private MqttClient _client;
        private int elbowSpeed, wristSpeed;

        public System.Action<MqttEntry> OnElbowValue, OnWristValue;

        void Start()
        {
            _client = new MqttClient(_clientIp);
            byte code = _client.Connect("unity_program");

            _client.MqttMsgPublished += client_MqttMsgPublished;
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //ushort publishId = _client.Publish("unity_topic",Encoding.UTF8.GetBytes("test_message"));

            ushort subscribeId = _client.Subscribe(new string[] { _wristTopic, _elbowTopic },
                    new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
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
                OnElbowValue?.Invoke(MessageToEntry(elbowValue));

                /*var newAngle = float.Parse(elbowValue.Split(',')[0]);

                if (newAngle != _elbowAngle)
                {
                    horizontal = (newAngle > _elbowAngle) ? 1f : -1f;


                    _elbowAngle = newAngle;
                }
                else
                {
                    horizontal = 0f;
                }*/


            }
            if (e.Topic == _wristTopic)
            {
                // Do anything with the value
                wristValue = Encoding.UTF8.GetString(e.Message);

                //Pass on the value to all event subscribers
                OnWristValue?.Invoke(MessageToEntry(wristValue));
            }
        }

        public void SetMotorSpeed(int speed, bool isElbow)
        {
            string topic = (isElbow) ? "motor_command_elbow" : "motor_command_wrist";

            PublishMessage(speed.ToString(), topic);
        }

        private ushort PublishMessage(string msg, string topic)
        {
            return _client.Publish(topic, Encoding.UTF8.GetBytes(msg));
        }

        private MqttEntry MessageToEntry(string msg)
        {
            var split = msg.Split(',');
            var value = float.Parse(split[0]);
            var time = split[1];

            return new MqttEntry(value, time);
        }
    }
}
