using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt
{
    public class mqtt_controller : MonoBehaviour
    {
        public string directionFromMindstorms;
        // Create some MQTT broker subscription

        public float speed = 0.01f;
        public float vertical = 0f;
        public float horizontal = 0f;
        public string elbowValue, wristValue;
        private float _elbowAngle, _wristAngle;

        private MqttClient client;
        private int elbowSpeed, wristSpeed;
        // Start is called before the first frame update
        void Start()
        {
            client = new MqttClient("192.168.0.101");
            byte code = client.Connect("unity_program");

            client.MqttMsgPublished += client_MqttMsgPublished;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            ushort publishId = client.Publish("unity_topic",
                   Encoding.UTF8.GetBytes("test_message"));

            ushort subscribeId = client.Subscribe(new string[] { "motor_value_wrist", "motor_value_elbow" },
                    new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Debug.Log("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.Log("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
            if (e.Topic == "motor_value_elbow")
            {
                // Do anything with the value
                elbowValue = Encoding.UTF8.GetString(e.Message);

                var newAngle = float.Parse(elbowValue.Split(',')[0]);

                if (newAngle != _elbowAngle )
                {
                    horizontal = (newAngle > _elbowAngle) ? 1f : -1f;


                    _elbowAngle = newAngle;
                }
                else
                {
                    horizontal = 0f;
                }


            }
            if (e.Topic == "motor_value_wrist")
            {
                // Do anything with the value
                wristValue = Encoding.UTF8.GetString(e.Message);
            }
        }

        public void SendMessage(bool elbow)
        {
            if (elbow)
            {
                elbowSpeed = (elbowSpeed == 0) ? 100 : 0;
                ushort publishId = client.Publish("motor_command_elbow",
                       Encoding.UTF8.GetBytes(elbowSpeed.ToString()));
            }
            else
            {
                wristSpeed = (wristSpeed == 0) ? 100 : 0;
                ushort publishId = client.Publish("motor_command_wrist",
                       Encoding.UTF8.GetBytes(wristSpeed.ToString()));
            }
        }

        // Update is called once per frame
        void Update()
        {


            Debug.Log(directionFromMindstorms);
            switch (directionFromMindstorms)
            {
                case "up":
                    vertical = 1.0f;
                    break;
                case "down":
                    vertical = -1.0f;
                    break;
                case "left":
                    horizontal = -1.0f;
                    break;
                case "right":
                    horizontal = 1.0f;
                    break;
                case "center":
                    vertical = 0f;
                    horizontal = 0f;
                    break;
                default:
                    //    Debug.Log("Default case");
                    break;
            }

            vertical *= speed;
            horizontal *= speed;
            transform.Translate(horizontal, vertical, 0);
        }
    }
}
