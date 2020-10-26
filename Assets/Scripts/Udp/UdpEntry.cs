using LogModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Udp
{
    public class UdpEntry : LogEntry
    {
        public float Value { get; private set; }
        public string UdpTimestamp { get; private set; }

        public string UnityTimestamp { get; private set; }

        public override string Header => "UDP Timestamp, Unity Timestamp, Angle";

        public override string Id => _id;

        public override string Name => _id;

        private string _id;

        public UdpEntry(string id, float value, string timestamp)
        {
            _id = id;
            Value = value;
            UdpTimestamp = timestamp;
            UnityTimestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
        }

        public UdpEntry(string id, float value, string mqtttime, string unitytime)
        {
            _id = id;
            Value = value;
            UdpTimestamp = mqtttime;
            UnityTimestamp = unitytime;
        }
        public override string ToCSV()
        {
            return $"\"{UdpTimestamp}\",\"{UnityTimestamp}\",\"{Value}\"";
        }

        public override string ToText()
        {
            return $"Value: {Value}, UDP TimeStamp: {UdpTimestamp}, Unity Timestamp: {UnityTimestamp}";
        }
    }
}
