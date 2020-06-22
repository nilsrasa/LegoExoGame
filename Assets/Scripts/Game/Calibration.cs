using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration
{
    private float _maxY, _minY, _maxX, _minX;
    private int setup;
    public void SetMaxWrist(float angle)
    {
        _maxX = angle;
        setup++;
    }

    public void SetMinWrist(float angle)
    {
        _minX = angle;
        setup++;
    }

    public void SetMaxElbow(float angle)
    {
        _maxY = angle;
        setup++;
    }

    public void SetMinElbow(float angle)
    {
        _minY = angle;
        setup++;
    }

    public float TranslateElbow (float angle)
    {
        angle = Mathf.Clamp(angle, _minY, _maxY);
        var middle = (_minY + _maxY) / 2f;
        var range = _maxY - middle;
        var dist = angle - middle;
        var pct = dist / range;

        return pct;
    }

    public float TranslateWrist(float angle)
    {
        angle = Mathf.Clamp(angle, _minX, _maxX);
        var middle = (_minX + _maxX) / 2f;
        var range = _maxX - middle;
        var dist = angle - middle;
        var pct = dist / range;

        return pct;
    }

}
