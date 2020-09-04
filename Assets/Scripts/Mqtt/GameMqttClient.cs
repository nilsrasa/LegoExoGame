using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mqtt
{
    public abstract class GameMqttClient:MonoBehaviour
    {
        public static System.Action<MqttEntry> OnElbowValue, OnWristValue;

        public abstract void Connect(string clientIp);

        public abstract void Close();

        public abstract void Nudge(NudgeDir nudgeDir);


    }

    public enum NudgeDir
    {
        up,
        down,
        left,
        right
    }
}
