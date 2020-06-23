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
        [SerializeField] private GameUI _gameUI;
        //InputMan
        //ObjectMan
        [SerializeField] private ObjectManager _objectManager;
        [SerializeField] private float _spawnInterval = 4f;
        //Patient
        //MqttMan
        [SerializeField] private DebugMqttMan _mqttManager;
        private float _elbowAngle, _wristAngle;

        //Game
        [SerializeField, Range(0.2f,5f)] private float _speed;
        [SerializeField] private Transform _hand;
        [SerializeField] private float _distanceFromMiddle = 1.5f;
        private int _score;

        //Calibration
        [SerializeField] private CalibrationController _calibrationController;
        private Calibration _calibration;

        private LogWriter _logWriter;

        

        public bool IsRunning { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            //Subscribing to the mqtt events
            _mqttManager.OnElbowValue += OnElbowValue;
            _mqttManager.OnWristValue += OnWristValue;

            //Sub to calibration controller event
            _calibrationController.OnCalibrationDone += OnCalibrationDone;

            //Sub to gameUi events
            GameUI.OnStartClick += NewGame;
            GameUI.OnCountedDown += OnCountedDown;

            //Subscribing the Cube event
            Cube.OnCollidedHand += OnCubeHit;

            //Initializing the object pool
            _objectManager.Init(4);

        }

        void Update()
        {
            if (IsRunning)
            {
                //Set Difficulty
                Cube.speed = _speed;

                //Move the hand
                var pct = _calibration.ElbowPercent(_elbowAngle);
                var pos = _hand.position;
                pos.y = _distanceFromMiddle * pct;
                pct = _calibration.WristPercent(_wristAngle);
                pos.x = _distanceFromMiddle * pct;

                _hand.position = pos;

                SpawnCount();
            }
        }

        #region Game
        private float _nextSpawn;
        private const float _zStart = 10f;
        public void NewGame()
        {
            //Init LogWriter
            _logWriter = new LogWriter(Application.persistentDataPath + "\\Logs\\");

            //Init MqttManager
            _mqttManager.Connect();

            //Init calibration
            _calibrationController.StartCalibration(_mqttManager);

        }

        public void StartGame()
        {
            IsRunning = true;
        }

        private void OnCalibrationDone(Calibration calibration)
        {
            _calibration = calibration;
            _gameUI.ShowCountdown(3);
        }

        private void OnCountedDown()
        {
            StartGame();
        }

        public void PauseGame()
        {
            IsRunning = false;
        }

        public void ResumeGame()
        {
            IsRunning = true;
        }

        private void SpawnCount()
        {
            _nextSpawn -= Time.deltaTime;

            if (_nextSpawn <= 0)
            {
                Spawn();
                _nextSpawn = _spawnInterval;
            }
        }

        private void Spawn()
        {
            var pos = Vector3.forward * _zStart;
            var dir = (CubeDirection)Random.Range(0, 4);

            switch (dir)
            {
                case CubeDirection.Up:
                    pos += Vector3.up * _distanceFromMiddle;
                    break;
                case CubeDirection.Down:
                    pos += Vector3.down * _distanceFromMiddle;
                    break;
                case CubeDirection.Left:
                    pos += Vector3.left * _distanceFromMiddle;
                    break;
                case CubeDirection.Right:
                    pos += Vector3.right * _distanceFromMiddle;
                    break;
            }


            var cube = _objectManager.SpawnCube(pos, Quaternion.identity);
            cube.Direction = dir;
        }

        private void OnCubeHit()
        {
            _score += Cube.points;
            _gameUI.UpdateScoreTxt(_score, Cube.points);
        }
        #endregion

        #region Mqtt
        private void OnElbowValue(MqttEntry entry)
        {
            //Save value
            _elbowAngle = entry.Value;

            //Log entry
            _logWriter.LogEntry(entry);
        }

        private void OnWristValue(MqttEntry entry)
        {
            //Save value
            _wristAngle = entry.Value;

            //Log entry
            _logWriter.LogEntry(entry);
        }
        #endregion
    }
}
