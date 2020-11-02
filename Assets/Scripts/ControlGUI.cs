using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ControlGUI : MonoBehaviour
    {
        private string _clientIp, _clientPort;
        private bool _isSafeModeOn;
        private int _gameDif;
        private const int MARGIN = 10;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {

            int height = 230;
            int width = 320;
            GUI.BeginGroup(new Rect(20, 20, width, height));

            //BackgroundBox
            //GUI.Box(new Rect(0, 0, width, height), ""); //Trick to reduce opacity
            GUI.Box(new Rect(0, 0, width, height), "Settings");

            //Patient Name Label
            int y = MARGIN * 3;
            int x = width / 2;
            GUI.Label(new Rect(x - 50, y, 100, 30), "Client:");
            //Ip inputfield
            y += 15 + MARGIN;
            string newIp = GUI.TextField(new Rect(x - 72, y, 100, 20), _clientIp);
            if (Validate(newIp))
            {
                if (_clientIp != newIp)
                {
                    _clientIp = newIp;
                    OnClientIpChanged(_clientIp);
                }
            }
            //Port inputfield
            string newPort = GUI.TextField(new Rect(x + 32, y, 40, 20), _clientPort);
            if (Validate(newPort))
            {
                if (_clientPort != newPort)
                {
                    _clientPort = newPort;
                    OnClientPortChanged(_clientPort);
                }
            }

            //Game diff label
            y += 20 + MARGIN;
            GUI.Label(new Rect(x - 50, y, 100, 30), "Difficulty:");
            //Diff grid
            y += 15 + MARGIN;
            int oldDif = _gameDif;
            _gameDif = GUI.Toolbar(new Rect(x - 120, y, 240, 30), _gameDif, Enum.GetNames(typeof(GameSettings.Difficulty)));
            if (oldDif != _gameDif) OnDifDropChanged(_gameDif);

            //Safe Mode
            y += 30 + MARGIN;
            _isSafeModeOn = GUI.Toggle(new Rect(x - 50, y, 100, 30), _isSafeModeOn, "SAFE MODE");

            //Controls
            y = height - 30 - MARGIN;
            //Reset button
            if (GUI.Button(new Rect(x - 80 - MARGIN, y, 80, 30), "Reset"))
            {
                //TODO reset
            }
            else if (GUI.Button(new Rect(x + MARGIN, y, 80, 30), "Submit"))
            {
                SaveSettings();
            }

            GUI.EndGroup();
        }

        private bool Validate(string id)
        {
            return true;
        }

        public void ApplySettings(GameSettings settings)
        {
            _clientIp = settings.ClientIp;
            _clientPort = settings.ClientPort.ToString();
            _isSafeModeOn = settings.SafeMode;
            _gameDif = (int)settings.GameDif;
            //_gameDifDrop.RefreshShownValue();
        }

        private void ResetSettings()
        {

        }

        private void SaveSettings()
        {
            GameController.gameSettings.SaveChanges();
            //UpdateUI();
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
    }
}
