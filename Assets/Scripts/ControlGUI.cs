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
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
