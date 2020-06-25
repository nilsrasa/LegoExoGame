using LogModule;
using System;

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
            UnityTimestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
        }

        public MqttEntry(string id, float value, string mqtttime, string unitytime)
        {
            _id = id;
            Value = value;
            MqttTimestamp = mqtttime;
            UnityTimestamp = unitytime;
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
