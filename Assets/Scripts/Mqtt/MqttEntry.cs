using LogModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mqtt
{
    public class MqttEntry : LogEntry
    {
        public float Value { get; private set; }
        public string Timestamp { get; private set; }
        public MqttEntry(float value, string timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }
        public override string ToCSV()
        {
            return $"\"{Value}\",\"{Timestamp}\"";
        }

        public override string ToText()
        {
            return $"Value: {Value}, TimeStamp: {Timestamp}";
        }
    }
}
