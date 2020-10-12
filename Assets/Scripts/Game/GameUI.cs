using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameUI : MonoBehaviour
    {
        #region Singleton Pattern
        public static GameUI Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
        #endregion

        [Header("Game Panel")]
        [SerializeField] private RectTransform _gamePanel;
        [SerializeField] private TextMeshProUGUI _scoreTxt;
        private const string SCORE = "Score: {0}";
        [Header("Main Menu Panel")]
        [SerializeField] private RectTransform _mainPanel;
        [SerializeField] private Button _startBtn, _settingsBtn;
        [Header("Settings Menu Panel")]
        [SerializeField] private RectTransform _settingsPanel;
        [SerializeField] private Button _resetBtn, _saveBtn;
        [SerializeField] private TMP_InputField _clientIpInput, _clientPortInput;
        [SerializeField] private TMP_Dropdown _gameDifDrop;
        [SerializeField] private Toggle _safeModeToggle;
        [Header("Pause Panel")]
        [SerializeField] private RectTransform _pausePanel;
        [Header("Count Panel")]
        [SerializeField] private RectTransform _countPanel;
        [SerializeField] private TextMeshProUGUI _countText;
        [Header("Instruction Panel")]
        [SerializeField] private RectTransform _instructionPanel;
        [SerializeField] private TextMeshProUGUI _instructionText;
        [Header("Game Over Panel")]
        [SerializeField] private RectTransform _gameoverPanel;
        [SerializeField] private TextMeshProUGUI _goScoreText, _goHitText, _goMissText;
        [SerializeField] private Button _goRestartBtn, _goNewBtn;
        private const string HIT = "Cubes hit: {0}", MISS = "Cubes missed: {0}";

        //Counting
        private int _count;
        private float _countTime;
        private const float COUNT_INTERVAL = 1f;
        public static event System.Action OnCountedDown;

        public static event System.Action OnStartClick, OnRestartClick, OnNewClick;
        

        private enum State
        {
            Menu,
            Game,
            Pause,
            Counting,
            Instructions,
            GameOver,
            Settings
        }
        private State _state;

        private void Start()
        {
            _state = State.Menu;
            UpdateUI();

            //Binding the buttons
            _startBtn.onClick.AddListener(StartGame);
            _goNewBtn.onClick.AddListener(() => {
                _state = State.Menu;
                UpdateUI();
            });
            _goRestartBtn.onClick.AddListener(RestartGame);
            _settingsBtn.onClick.AddListener(() =>
            {
                _state = State.Settings;
                UpdateUI();
                ApplySettings(GameController.gameSettings);
            });
            _resetBtn.onClick.AddListener(ResetSettings);
            _saveBtn.onClick.AddListener(SaveSettings);

            _safeModeToggle.onValueChanged.AddListener((bool b) => GameController.gameSettings.SetSafeMode(b));

            //Binding settings UI elelments
            _clientIpInput.onEndEdit.AddListener(OnClientIpChanged);
            _clientPortInput.onEndEdit.AddListener(OnClientPortChanged);
            _gameDifDrop.onValueChanged.AddListener(OnDifDropChanged);
        }

        private void Update()
        {
            //Whenever the state is Counting, count the time
            if (_state == State.Counting)
            {
                _countTime -= Time.deltaTime;
                if (_countTime <= 0)
                {
                    _count--;
                    _countTime = COUNT_INTERVAL;
                    _countText.text = (_count > 0) ? _count.ToString() : "Start";

                    if (_count < 0)
                    {
                        OnCountedDown?.Invoke();

                        _state = State.Game;
                        UpdateUI();
                    }
                }
            }
        }
        /// <summary>
        /// Call this to start a countdown
        /// </summary>
        /// <param name="count">The starting count, excluding "Start"</param>
        public void ShowCountdown(int count)
        {
            _count = count + 1;
            _state = State.Counting;
            UpdateUI();
        }

        /// <summary>
        /// Makes sure the right panels are visible depending on the State.
        /// </summary>
        private void UpdateUI()
        {
            _gamePanel.gameObject.SetActive(_state == State.Game);
            _pausePanel.gameObject.SetActive(_state == State.Pause);
            _mainPanel.gameObject.SetActive(_state == State.Menu);
            _countPanel.gameObject.SetActive(_state == State.Counting);
            _instructionPanel.gameObject.SetActive(_state == State.Instructions);
            _gameoverPanel.gameObject.SetActive(_state == State.GameOver);
            _settingsPanel.gameObject.SetActive(_state == State.Settings);
        }

        private void StartGame()
        {
            _state = State.Game;
            UpdateUI();

            OnStartClick?.Invoke();
        }

        private void RestartGame()
        {
            _state = State.Game;
            UpdateUI();

            OnRestartClick?.Invoke();
        }

        public void ShowInstructions(string msg)
        {
            _instructionText.text = msg;
            _state = State.Instructions;
            UpdateUI();
        }

        public void ClearInstructions()
        {
            _state = State.Game;
            UpdateUI();
        }

        public void ShowPauseScreen()
        {
            _state = State.Pause;
            UpdateUI();
        }

        public void HidePauseScreen()
        {
            _state = State.Game;
            UpdateUI();
        }

        public void ShowGameOverScreen(int hit, int miss, int score)
        {
            _goHitText.text = string.Format(HIT, hit);
            _goMissText.text = string.Format(MISS, miss);
            _goScoreText.text = string.Format(SCORE, score);

            _state = State.GameOver;
            UpdateUI();
        }

        #region Settings
        public void ApplySettings(GameSettings settings)
        {
            _clientIpInput.text = settings.ClientIp;
            _clientPortInput.text = settings.ClientPort.ToString();
            _safeModeToggle.isOn = settings.SafeMode;
            _gameDifDrop.value = (int)settings.GameDif;
            //_gameDifDrop.RefreshShownValue();
        }

        private void ResetSettings()
        {
            
        }

        private void SaveSettings()
        {
            GameController.gameSettings.SaveChanges();
            _state = State.Menu;
            UpdateUI();
        }

        private void OnClientIpChanged(string input)
        {
            GameController.gameSettings.SetClientIp(input);
        }

        private void OnClientPortChanged(string port)
        {
            GameController.gameSettings.SetClientPort(port);
        }

        private void OnDifDropChanged(int val)
        {
            GameController.gameSettings.SetDifficulty(val);
        }

        #endregion

        #region Score
        private void AddScore(int points)
        {


            //todo: do something visually cool
        }

        private void SubstractScore(int points)
        {

        }

        /// <summary>
        /// Call this to update the score text on the GUI
        /// </summary>
        /// <param name="score">The current score</param>
        /// <param name="points">The amount of points the score changed by</param>
        public void UpdateScoreTxt(int score, int points)
        {
            _scoreTxt.text = string.Format(SCORE, score);

            if (points > 0) //If we're adding
            {
                AddScore(points);
            }
            else if (points < 0) //If we're substracting
            {
                SubstractScore(points);
            }
        }
        #endregion
    }
}
