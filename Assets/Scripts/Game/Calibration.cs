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

    public float ElbowPercent (float angle)
    {
        var percent = Percent(angle, _minY, _maxY);
        var poly = Polynomial(percent);

        return percent;
    }

    public float WristPercent(float angle)
    {
        var percent = Percent(angle, _minX, _maxX);
        var poly = Polynomial(percent);

        return percent;
    }

    private float Percent(float angle, float min, float max)
    {
        angle = Mathf.Clamp(angle, min, max);
        var middle = (min + max) / 2f;
        var range = max - middle;
        var dist = angle - middle;
        var pct = dist / range;

        return pct;
    }

    private float Polynomial(float pct)
    {
        var poly = (0.1f * Mathf.Pow(pct * 10, 2)) / 10;
        poly = (pct >= 0) ? poly : -poly;

        return poly;
    }

}
