using Game;
using Mqtt;
using UnityEngine;

namespace Testing
{
    public class DebugMqttMan : GameMqttClient
    {
        public string elbowValue, wristValue;

        [SerializeField] private string _clientIp = "192.168.0.101";
        [SerializeField] private string _elbowTopic = "motor_value_elbow";
        [SerializeField] private string _wristTopic = "motor_value_wrist";
        [SerializeField] private string _elbowCommand = "motor_command_elbow";
        [SerializeField] private string _wristCommand = "motor_command_wrist";

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

        public override void Connect(string clientIp)
        {
            Log("Connected to ip: "+clientIp);
            isConnected = true;
        }

        public override void Close()
        {
            Log("Closed");
            isConnected = false;
        }

        public void ReceiveValue(float val, string time, bool isElbow)
        {
            Log("Received val: " + val + " at: " + time + " isElbow: " + isElbow);

            if (isElbow)
            {
                elbowValue = val.ToString();
                OnElbowValue?.Invoke(new MqttEntry("elbow", val, time));
            }
            else
            {
                wristValue = val.ToString();
                OnWristValue?.Invoke(new MqttEntry("wrist", val, time));
            }
        }

        public void SetMotorSpeed(int speed, bool isElbow)
        {
            string topic = (isElbow) ? _elbowCommand : _wristCommand;

            Log(topic + " speed: " + speed);
        }

        public override void Nudge(NudgeDir dir)
        {
            Log("nudging to the " + dir.ToString());
        }

        private void Log(string entry)
        {
            Debug.Log("MqttMan:: " + entry);
        }
    }
}
