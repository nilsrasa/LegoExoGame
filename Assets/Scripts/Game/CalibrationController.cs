using Game;
using Mqtt;
using System.Collections;
using System.Collections.Generic;
using Testing;
using UnityEngine;

public class CalibrationController : MonoBehaviour
{
    private Calibration _calibration;
    private MqttManager _mqttManager;
    private float _elbowAngle;
    private float _wristAngle;

    public event System.Action<Calibration> OnCalibrationDone;

    public void StartCalibration(MqttManager mqttManager)
    {
        _mqttManager = mqttManager;
        _mqttManager.OnElbowValue += OnElbowValue;
        _mqttManager.OnWristValue += OnWristValue;


        _calibration = new Calibration();

        StartCoroutine(Calibrate());
    }

    private IEnumerator Calibrate()
    {
        GameUI.Instance.ShowInstructions("Move your arm to it's lowest position");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMinElbow(_elbowAngle);
        yield return new WaitForSeconds(.2f);

        GameUI.Instance.ShowInstructions("Move your arm to it's highest position");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMaxElbow(_elbowAngle);
        yield return new WaitForSeconds(.2f);

        GameUI.Instance.ShowInstructions("Turn your wrist to the left");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMinWrist(_wristAngle);
        yield return new WaitForSeconds(.2f);

        GameUI.Instance.ShowInstructions("Turn your wrist to the right");
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Return));
        _calibration.SetMaxWrist(_wristAngle);

        GameUI.Instance.ClearInstructions();
        OnCalibrationDone?.Invoke(_calibration);
    }

    private void OnElbowValue(MqttEntry entry)
    {
        _elbowAngle = entry.Value;
    }

    private void OnWristValue(MqttEntry entry)
    {
        _wristAngle = entry.Value;
    }
}
