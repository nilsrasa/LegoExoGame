using Mqtt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class DebugMqttMan : MonoBehaviour
    {
        public string elbowValue, wristValue;

        public System.Action<MqttEntry> OnElbowValue, OnWristValue;

        void Start()
        {
            
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
            string topic = (isElbow) ? "motor_command_elbow" : "motor_command_wrist";

            Debug.Log(topic + " speed: " + speed);
        }
    }
}
