using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameSettings
    {
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard
        }

        public string ClientIp { get; private set; }
        public Int32 ClientPort { get; private set; }
        public Difficulty GameDif { get; private set; }
        public float GameSpeed { get; private set; }
        public float CubeSpacing { get; private set; }
        public float DistFromMiddle { get; private set; }
        public int CubeCount { get; private set; }
        public bool SafeMode { get; private set; }

        private const string CLIENTIP = "ClientIP", CLIENTPORT = "ClientPort", GAMEDIF = "GameDif", GAMESPEED = "GameSpeed", CUBESPACING = "CubeSpacing", DISTFROMMIDDLE = "Dist", CUBECOUNT = "CubeCount", EXISTS = "Exists", SAFEMODE = "SafeMode";

        public GameSettings()
        {
            if (PlayerPrefs.HasKey(EXISTS))
            {
                ClientIp = PlayerPrefs.GetString(CLIENTIP);
                ClientPort = PlayerPrefs.GetInt(CLIENTPORT);
                SafeMode = PlayerPrefs.GetInt(SAFEMODE) == 1;
                SetDifficulty(PlayerPrefs.GetInt(GAMEDIF));
            }
            else
            {
                ClientIp = "192.168.0.101";
                ClientPort = 5011;
                SafeMode = false;
                SetDifficulty(Difficulty.Normal);
                PlayerPrefs.SetString(CLIENTIP, ClientIp);
                PlayerPrefs.SetInt(CLIENTPORT, ClientPort);
                PlayerPrefs.SetInt(SAFEMODE, SafeMode ? 1 : 0);
                PlayerPrefs.SetInt(GAMEDIF, (int)GameDif);
                PlayerPrefs.SetInt(EXISTS, 1);
                PlayerPrefs.Save();
            }
        }
        
        public void SetDifficulty(int difficulty)
        {
            SetDifficulty((Difficulty)difficulty);
        }

        public void SetDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    GameSpeed = .8f;
                    CubeSpacing = 5f;
                    DistFromMiddle = 2.8f;
                    CubeCount = 10;
                    break;
                case Difficulty.Normal:
                    GameSpeed = 1f;
                    CubeSpacing = 4f;
                    DistFromMiddle = 3.5f;
                    CubeCount = 20;
                    break;
                case Difficulty.Hard:
                    GameSpeed = 1.8f;
                    CubeSpacing = 3f;
                    DistFromMiddle = 4.5f;
                    CubeCount = 30;
                    break;
            }

            GameDif = difficulty;
            PlayerPrefs.SetInt(GAMEDIF, (int)GameDif);
        }

        public void SetClientIp(string ip)
        {
            ClientIp = ip;
            PlayerPrefs.SetString(CLIENTIP, ClientIp);
        }

        public void SetClientPort(string port)
        {
            ClientPort = Int32.Parse(port);
            PlayerPrefs.SetInt(CLIENTPORT, ClientPort);
        }

        public void SetSafeMode(bool b)
        {
            SafeMode = b;
            PlayerPrefs.SetInt(SAFEMODE, SafeMode ? 1 : 0);
        }

        public void SaveChanges()
        {
            PlayerPrefs.Save();
        }
    }
}
