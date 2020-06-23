using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
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
        [SerializeField] private Button _startBtn;
        [Header("Pause Panel")]
        [SerializeField] private RectTransform _pausePanel;
        [Header("Count Panel")]
        [SerializeField] private RectTransform _countPanel;
        [SerializeField] private TextMeshProUGUI _countText;
        [Header("Instruction Panel")]
        [SerializeField] private RectTransform _instructionPanel;
        [SerializeField] private TextMeshProUGUI _instructionText;

        //Counting
        private int _count;
        private float _countTime;
        private const float COUNT_INTERVAL = 1f;
        public static event System.Action OnCountedDown;

        public static event System.Action OnStartClick;
        

        private enum State
        {
            Menu,
            Game,
            Pause,
            Counting,
            Instructions
        }
        private State _state;

        private void Start()
        {
            _state = State.Menu;
            UpdateUI();


            _startBtn.onClick.AddListener(StartGame);
        }

        private void Update()
        {
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

        public void ShowCountdown(int count)
        {
            _count = count + 1;
            _state = State.Counting;
            UpdateUI();
        }


        private void UpdateUI()
        {
            _gamePanel.gameObject.SetActive(_state == State.Game);
            _pausePanel.gameObject.SetActive(_state == State.Pause);
            _mainPanel.gameObject.SetActive(_state == State.Menu);
            _countPanel.gameObject.SetActive(_state == State.Counting);
            _instructionPanel.gameObject.SetActive(_state == State.Instructions);
        }

        private void StartGame()
        {
            _state = State.Game;
            UpdateUI();

            OnStartClick?.Invoke();
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

        #region Score
        private void AddScore(int points)
        {


            //todo: do something visually cool
        }

        private void SubstractScore(int points)
        {

        }

        public void UpdateScoreTxt(int score, int points)
        {
            _scoreTxt.text = string.Format(SCORE, score);

            if (points > 0) //If we're adding
            {
                AddScore(points);
            }
            else //If we're substracting
            {
                SubstractScore(points);
            }
        }
        #endregion
    }
}
