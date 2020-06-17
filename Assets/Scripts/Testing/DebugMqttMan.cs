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

        public void Connect()
        {
            Log("Connected");
        }

        public void Close()
        {
            Log("Closed");
        }

        public void ReceiveValue(float val, string time, bool isElbow)
        {
            var entry = new MqttEntry(val, time);

            if (isElbow)
                OnElbowValue?.Invoke(entry);
            else
                OnWristValue?.Invoke(entry);
        }

        public void SetMotorSpeed(int speed, bool isElbow)
        {
            string topic = (isElbow) ? _elbowCommand : _wristCommand;

            Log(topic + " speed: " + speed);
        }

        private void Log(string entry)
        {
            Debug.Log("MqttMan:: " + entry);
        }
    }
}
