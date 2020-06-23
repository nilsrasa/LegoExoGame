using Mqtt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class DebugMqttMan : MonoBehaviour
    {
        public string elbowValue, wristValue;

        [SerializeField] private string _clientIp = "192.168.0.101";
        [SerializeField] private string _elbowTopic = "motor_value_elbow";
        [SerializeField] private string _wristTopic = "motor_value_wrist";
        [SerializeField] private string _elbowCommand = "motor_command_elbow";
        [SerializeField] private string _wristCommand = "motor_command_wrist";

        public event System.Action<MqttEntry> OnElbowValue, OnWristValue;
        private float _elbowValue, _wristValue;
        private float moveAmount = 6f;
        public bool isConnected { get; private set; }


        private void Update()
        {
            if (isConnected)
            {
                /*if (Input.GetKey(KeyCode.A))
                {
                    _wristValue -= moveAmount;
                    ReceiveValue(_wristValue, Time.time.ToString(), false);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    _wristValue += moveAmount;
                    ReceiveValue(_wristValue, Time.time.ToString(), false);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    _elbowValue += moveAmount;
                    ReceiveValue(_elbowValue, Time.time.ToString(), true);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    _elbowValue -= moveAmount;
                    ReceiveValue(_elbowValue, Time.time.ToString(), true);
                }*/

                ReceiveValue(Input.mousePosition.x, Time.time.ToString(), false);
                ReceiveValue(Input.mousePosition.y, Time.time.ToString(), true);
            }
        }

        public void Connect()
        {
            Log("Connected");
            isConnected = true;
        }

        public void Close()
        {
            Log("Closed");
            isConnected = false;
        }

        public void ReceiveValue(float val, string time, bool isElbow)
        {
            Log("Received val: " + val + " at: " + time + " isElbow: " + isElbow);

            if (isElbow)
                OnElbowValue?.Invoke(new MqttEntry("elbow", val, time));
            else
                OnWristValue?.Invoke(new MqttEntry("wrist", val, time));
        }

        public void SetMotorSpeed(int speed, bool isElbow)
        {
            string topic = (isElbow) ? _elbowCommand : _wristCommand;

            Log(topic + " speed: " + speed);
        }

        private void Log(string entry)
        {
            //Debug.Log("MqttMan:: " + entry);
        }
    }
}
