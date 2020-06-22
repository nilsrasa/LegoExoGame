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
        private float _horizontal, _vertical;
        [SerializeField] private float _speed;
        [SerializeField] private Transform _hand;
        [SerializeField] private float _distanceFromMiddle = 1.5f;
        private Calibration _calibration;

        private LogWriter _logWriter;

        

        public bool IsRunning { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            //Subscribing to the mqtt events
            _mqttManager.OnElbowValue += OnElbowValue;
            _mqttManager.OnWristValue += OnWristValue;

            //Initializing the object pool
            _objectManager.Init(4);

        }

        void Update()
        {
            if (IsRunning)
            {
                /*var xSpeed = _horizontal * _speed * Time.deltaTime;
                var ySpeed = _vertical * _speed * Time.deltaTime;
                _horizontal = 0;
                _vertical = 0;

                _hand.Translate(xSpeed, ySpeed, 0);*/

                var pct = _calibration.TranslateElbow(_elbowAngle);
                var pos = _hand.position;
                pos.y = _distanceFromMiddle * pct;
                pct = _calibration.TranslateWrist(_wristAngle);
                pos.x = _distanceFromMiddle * pct;

                _hand.position = pos;

                SpawnCount();
            }
        }

        #region Game
        private float _nextSpawn;
        private const float _zStart = 10f;
        private const float _square = 4f;
        public void NewGame()
        {
            //Init MqttManager
            _mqttManager.Connect();

            //Init LogWriter
            _logWriter = new LogWriter(Application.persistentDataPath + "\\Logs\\");

            //Start
            //IsRunning = true;
            CalibrationSequence();
        }

        public void CalibrationSequence()
        {
            _calibration = new Calibration();

            StartCoroutine(Calibrate());
        }

        private IEnumerator Calibrate()
        {
            Debug.Log("Set min elbow");
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
            _calibration.SetMinElbow(_elbowAngle);
            yield return new WaitForSeconds(1f);

            Debug.Log("Set max elbow");
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
            _calibration.SetMaxElbow(_elbowAngle);
            yield return new WaitForSeconds(1f);

            Debug.Log("Set min wrist");
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
            _calibration.SetMinWrist(_wristAngle);
            yield return new WaitForSeconds(1f);

            Debug.Log("Set max wrist");
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
            _calibration.SetMaxWrist(_wristAngle);

            IsRunning = true;
        }

        public void PauseGame()
        {

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
            var rot = Quaternion.identity;

            switch (dir)
            {
                case CubeDirection.Up:
                    //rot = Quaternion.identity;
                    pos += Vector3.up * _distanceFromMiddle;
                    break;
                case CubeDirection.Down:
                    rot = Quaternion.Euler(0f, 0f, 180f);
                    pos += Vector3.down * _distanceFromMiddle;
                    break;
                case CubeDirection.Left:
                    rot = Quaternion.Euler(0f, 0f, 90f);
                    pos += Vector3.left * _distanceFromMiddle;
                    break;
                case CubeDirection.Right:
                    rot = Quaternion.Euler(0f, 0f, 270f);
                    pos += Vector3.right * _distanceFromMiddle;
                    break;
            }


            var cube = _objectManager.SpawnCube(pos, rot);
            //cube.Direction = dir;
        }
        #endregion

        #region Mqtt
        private void OnElbowValue(MqttEntry entry)
        {
            //Handle value
            //HandleAngleChange(entry.Value, ref _elbowAngle, ref _vertical);
            _elbowAngle = entry.Value;

            //Log entry
            _logWriter.LogEntry(entry);
        }

        private void OnWristValue(MqttEntry entry)
        {
            //Handle value
            //HandleAngleChange(entry.Value, ref _wristAngle, ref _horizontal);
            _wristAngle = entry.Value;

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
