using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Udp;
using UnityEngine;

public class ExoUdpHost : UdpHost
{
    public static Action<UdpEntry> OnElbowValue, OnWristValue;

    [SerializeField] private float _elbowValue, _wristValue;
    private const string ELBOW_ID = "elbow", WRIST_ID = "wrist";

    public override void MessageReceived(string message)
    {
        base.MessageReceived(message);

        var data = message.Split(',');
        var unitytime = DateTime.Now.ToString("HH:mm:ss.ffffff");

        if (data.Length == 3)
        {
            if (float.TryParse(data[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _elbowValue))
            {
                OnElbowValue?.Invoke(new UdpEntry(ELBOW_ID, _elbowValue, data[2], unitytime));
            }

            if (float.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _wristValue))
            {
                OnWristValue?.Invoke(new UdpEntry(WRIST_ID, _wristValue, data[2], unitytime));
            }
        }
    }

    public void Nudge(int i)
    {
        Nudge((NudgeDir)i);
    }

    public void Nudge(NudgeDir dir)
    {
        SendMsg(dir.ToString());
    }
}
