using LogModule;
using Mqtt;
using System.Collections;
using System.Collections.Generic;
using Testing;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        //GameUI
        //InputMan
        //ObjectMan
        //Game
        //Patient
        //MqttMan
        [SerializeField] private DebugMqttMan _mqttManager;
        private float _elbowAngle, _wristAngle;

        //Game
        private float _horizontal, _vertical;
        [SerializeField] private float _speed;
        [SerializeField] private Transform _hand;

        private LogWriter _logWriter;

        public bool IsRunning { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            //Subscribing to the mqtt events
            _mqttManager.OnElbowValue += OnElbowValue;
            _mqttManager.OnWristValue += OnWristValue;




        }

        // Update is called once per frame
        void Update()
        {
            if (IsRunning)
            {
                var xSpeed = _horizontal * _speed * Time.deltaTime;
                var ySpeed = _vertical * _speed * Time.deltaTime;
                _horizontal = 0;
                _vertical = 0;

                _hand.Translate(xSpeed, ySpeed, 0);
            }
        }

        private void NewGame()
        {
            //Init MqttManager
            _mqttManager.Connect();

            //Init LogWriter
            _logWriter = new LogWriter("path");
        }

        #region Mqtt
        private void OnElbowValue(MqttEntry entry)
        {
            //Handle value
            HandleAngleChange(entry.Value, ref _elbowAngle, ref _vertical);

            //Log entry
            _logWriter.LogEntry(entry);
        }

        private void OnWristValue(MqttEntry entry)
        {
            //Handle value
            HandleAngleChange(entry.Value, ref _wristAngle, ref _horizontal);

            //Log entry
            _logWriter.LogEntry(entry);
        }

        private void HandleAngleChange(float newAngle, ref float oldAngle, ref float side)
        {
            var dif = newAngle - oldAngle;
            oldAngle = newAngle;

            side += dif;
        }
        #endregion
    }
}
