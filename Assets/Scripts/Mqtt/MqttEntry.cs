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

        public override string Header => "Timestamp, Angle";

        public override string Id => _id;

        public override string Name => _id;

        private string _id;

        public MqttEntry(string id, float value, string timestamp)
        {
            _id = id;
            Value = value;
            Timestamp = timestamp;
        }
        public override string ToCSV()
        {
            return $"\"{Timestamp}\",\"{Value}\"";
        }

        public override string ToText()
        {
            return $"Value: {Value}, TimeStamp: {Timestamp}";
        }
    }
}
