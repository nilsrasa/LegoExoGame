using LogModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mqtt
{
    public class MqttEntry : LogEntry
    {
        public float Value { get; private set; }
        public string MqttTimestamp { get; private set; }

        public string UnityTimestamp { get; private set; }

        public override string Header => "Mqtt Timestamp, Unity Timestamp, Angle";

        public override string Id => _id;

        public override string Name => _id;

        private string _id;

        public MqttEntry(string id, float value, string timestamp)
        {
            _id = id;
            Value = value;
            MqttTimestamp = timestamp;
            UnityTimestamp = DateTime.Now.ToString("hh:mm:ss.ffff");
        }
        public override string ToCSV()
        {
            return $"\"{MqttTimestamp}\",\"{UnityTimestamp}\",\"{Value}\"";
        }

        public override string ToText()
        {
            return $"Value: {Value}, Mqtt TimeStamp: {MqttTimestamp}, Unity Timestamp: {UnityTimestamp}";
        }
    }
}
