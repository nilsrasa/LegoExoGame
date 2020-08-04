using LogModule;
using Mqtt;
using Testing;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [Header("External")]
        //GameUI
        [SerializeField] private GameUI _gameUI;
        //ObjectMan
        [SerializeField] private ObjectManager _objectManager;
        
        //MqttMan
        [SerializeField] private DebugMqttMan _mqttManager;
        private float _elbowAngle, _wristAngle;

        //Game
        [Header("Game objects")]
        [SerializeField] private Transform _hand;
        [SerializeField] private GameObject _nudgeTrigger;
        [SerializeField] private Grid _grid;
        [Header("Game Settings")]
        [SerializeField, Range(0.2f,5f)] private float _speed;
        [SerializeField] private float _spawnSpacing = 4f;
        [SerializeField] private float _distanceFromMiddle = 1.5f;
        [SerializeField] private int _cubeCount = 25;
        private int _spawnedCubes, _destroyedCubes;
        private int _hits, _misses;
        private float _spawnInterval;
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
            GameUI.OnRestartClick += RestartGame;
            GameUI.OnCountedDown += OnCountedDown;

            //Subscribing the Cube event
            Cube.OnCollidedHand += OnCubeHitHand;
            Cube.OnNudgeTrigger += OnNudgeTrigger;
            Cube.OnCollided += OnCubeHit;
            Cube.OnDisable += OnCubeDisabled;

            //Initializing the object pool
            _objectManager.Init(4);

            //Initializing the grid
            _grid.Init(_distanceFromMiddle, _zStart);

        }

        void Update()
        {
            if (IsRunning)
            {
                //Set Difficulty
                Cube.speed = _speed;
                _spawnInterval = _spawnSpacing / _speed;
                //Move the nudgetrigger accordingly
                var nudgerPosition = _nudgeTrigger.transform.localPosition;
                nudgerPosition.z = _spawnSpacing * .40f;
                _nudgeTrigger.transform.localPosition = nudgerPosition;

                //Move the ball(_hand) according to exo angles
                var pct = _calibration.ElbowPercent(_elbowAngle);
                var pos = _hand.position;
                pos.y = _distanceFromMiddle * pct;
                pct = _calibration.WristPercent(_wristAngle);
                pos.x = _distanceFromMiddle * pct;

                _hand.position = pos;

                SpawnCount();

                if (Input.GetKeyDown(KeyCode.P))
                {
                    PauseGame();
                }
            }
            else
            {
                if(Time.timeScale == 0)//If paused. TODO: Might have to use a statemachine instead?
                {
                    if (Input.GetKeyDown(KeyCode.P))
                        ResumeGame();
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Application.Quit();
                    }
                }
            }
        }

        private void OnDestroy()
        {
            StopGame();
        }

        #region Game
        private float _nextSpawn;
        private const float _zStart = 10f;
        public void NewGame()
        {
            ResetVars();

            //Init LogWriter
            _logWriter = new LogWriter(Application.persistentDataPath + "\\Logs\\" + System.DateTime.Now.ToString("dd-MM-yyyy HH'h'mm'm'ss's'") + "\\");

            //Init MqttManager
            _mqttManager.Connect();

            //Init calibration
            _calibrationController.StartCalibration(_mqttManager);

        }

        /// <summary>
        /// Called to start the gamecontroller
        /// </summary>
        public void StartGame()
        {
            IsRunning = true;
        }

        /// <summary>
        /// Call this when the calibration is done.
        /// </summary>
        /// <param name="calibration">The calibration object created during calibration</param>
        private void OnCalibrationDone(Calibration calibration)
        {
            _calibration = calibration;
            _gameUI.ShowCountdown(3);
        }

        /// <summary>
        /// When the count down is done.
        /// </summary>
        private void OnCountedDown()
        {
            StartGame();
        }

        /// <summary>
        /// Used to pause the gamecontroller.
        /// </summary>
        public void PauseGame()
        {
            Time.timeScale = 0;
            IsRunning = false;

            _gameUI.ShowPauseScreen();
        }

        /// <summary>
        /// Used to resume the gamecontroller
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = 1;
            IsRunning = true;

            _gameUI.HidePauseScreen();
        }

        /// <summary>
        /// when the game is over
        /// </summary>
        public void GameOver()
        {
            IsRunning = false;

            //Log Result
            _logWriter.LogEntry(new GameEntry(_score, _hits, _misses));

            //Update UI
            _gameUI.ShowGameOverScreen(_hits, _misses, _score);
        }

        public void RestartGame()
        {
            ResetVars();

            _gameUI.ShowCountdown(3);
        }

        private void ResetVars()
        {
            _spawnedCubes = 0;
            _destroyedCubes = 0;
            _hits = 0;
            _misses = 0;
            _score = 0;
            _gameUI.UpdateScoreTxt(_score, 0);
        }

        /// <summary>
        /// Used to stop the gamecontroller.
        /// Here everything is closed down.
        /// </summary>
        public void StopGame()
        {
            IsRunning = false;
            _mqttManager.Close();
            _logWriter.Close();
        }

        /// <summary>
        /// The spawn counter, called each Update().
        /// </summary>
        private void SpawnCount()
        {
            _nextSpawn -= Time.deltaTime;

            if (_nextSpawn <= 0 && _spawnedCubes < _cubeCount)
            {
                Spawn();
                _nextSpawn = _spawnInterval;
            }
        }

        /// <summary>
        /// Called when a cube needs to spawn.
        /// The direction is randomized here.
        /// </summary>
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
            _spawnedCubes++;
        }

        /// <summary>
        /// Called when the player hits a cube.
        /// </summary>
        private void OnCubeHitHand()
        {
            _hits++;
            _score += Cube.points;
            _gameUI.UpdateScoreTxt(_score, Cube.points);
        }

        /// <summary>
        /// Called when the cube hits anything else but the player hand.
        /// </summary>
        private void OnCubeHit()
        {
            _misses++;
            _score -= Cube.penalty;
            _gameUI.UpdateScoreTxt(_score, -Cube.penalty);
        }

        /// <summary>
        /// Called whenever a cube is disabled (hit or missed).
        /// </summary>
        /// <param name="cube"></param>
        private void OnCubeDisabled(Cube cube)
        {
            _destroyedCubes++;
            if (_destroyedCubes == _spawnedCubes)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Called to send a nudge to the player
        /// </summary>
        /// <param name="cube"></param>
        private void OnNudgeTrigger(Cube cube)
        {
            //Getting the ball x and y coordinates on grid where 0 is left down corner
            var hor = _hand.position.x + _distanceFromMiddle;
            var ver = _hand.position.y + _distanceFromMiddle;
            //Same with the cube x and y
            var cubeH = cube.transform.position.x + _distanceFromMiddle;
            var cubeV = cube.transform.position.y + _distanceFromMiddle;
            //Calculating the distance between the ball and the cube
            var distH = Mathf.Abs(hor - cubeH);
            var distV = Mathf.Abs(ver - cubeV);
            var triggerDist = _distanceFromMiddle * .4f;
            //If the distance is over the threshold
            if (distH > triggerDist)
            {
                //if the cube is on the right
                if(cubeH > _distanceFromMiddle)
                    _mqttManager.Nudge(NudgeDir.right);
                //if the cube is on the left
                else
                    _mqttManager.Nudge(NudgeDir.left);
            }
            //If the distance is over the threshold
            if (distV > triggerDist)
            {
                //if the cube is on the top
                if(cubeV > _distanceFromMiddle)
                    _mqttManager.Nudge(NudgeDir.up);
                //if the cube is on the bottom
                else
                    _mqttManager.Nudge(NudgeDir.down);
            }

            Debug.Log("Nudge was determined from distH:" + distH + " distV:" + distV + " cubeDir:" + cube.Direction.ToString());
            
            /*switch (cube.Direction)
            {
                case CubeDirection.Up:
                    if (distV >= triggerDist)
                        _mqttManager.Nudge(NudgeDir.up);
                    break;
                case CubeDirection.Down:
                    _mqttManager.Nudge(NudgeDir.down);
                    break;
                case CubeDirection.Left:
                    _mqttManager.Nudge(NudgeDir.left);
                    break;
                case CubeDirection.Right:
                    _mqttManager.Nudge(NudgeDir.right);
                    break;
            }*/
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
