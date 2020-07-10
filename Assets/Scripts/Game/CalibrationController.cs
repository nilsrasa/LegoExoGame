using Game;
using Mqtt;
using System.Collections;
using System.Collections.Generic;
using Testing;
using UnityEngine;

public class CalibrationController : MonoBehaviour
{
    private Calibration _calibration;
    private DebugMqttMan _mqttManager;
    private float _elbowAngle;
    private float _wristAngle;

    public event System.Action<Calibration> OnCalibrationDone;

    public void StartCalibration(DebugMqttMan mqttManager)
    {
        //Subscribing to the mqttManager events
        _mqttManager = mqttManager;
        _mqttManager.OnElbowValue += OnElbowValue;
        _mqttManager.OnWristValue += OnWristValue;

        //Instantiating a new Calibration object
        _calibration = new Calibration();

        //Start the calibration coroutine
        StartCoroutine(Calibrate());
    }

    private IEnumerator Calibrate()
    {
        //Set the minimum angle values
        GameUI.Instance.ShowInstructions("Strech your arm with your palm facing down.");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMinElbow(_elbowAngle);
        _calibration.SetMinWrist(_wristAngle);
        yield return new WaitForSeconds(.2f);
        
        //Set the maximum angle values
        GameUI.Instance.ShowInstructions("Bend your arm with your palm facing behind you.");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMaxElbow(_elbowAngle);
        _calibration.SetMaxWrist(_wristAngle);
        
        //Clear GUI and invoke the event
        GameUI.Instance.ClearInstructions();
        OnCalibrationDone?.Invoke(_calibration);
    }

    #region MQTT
    private void OnElbowValue(MqttEntry entry)
    {
        _elbowAngle = entry.Value;
    }

    private void OnWristValue(MqttEntry entry)
    {
        _wristAngle = entry.Value;
    }
    #endregion
}
