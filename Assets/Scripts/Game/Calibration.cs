using UnityEngine;
/// <summary>
/// Represents a 2-axis calibration.
/// </summary>
public class Calibration
{
    private float _maxY, _minY, _maxX, _minX;
    private int _dirX, _dirY;
    private int setup;
    public void SetMaxWrist(float angle)
    {
        _maxX = angle;
        _dirX = 1;
        if (_minX > _maxX)
        {
            var min = _maxX;
            _maxX = _minX;
            _minX = min;
            _dirX = -1;
        }
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
        _dirY = 1;
        if (_minY > _maxY)
        {
            var min = _maxY;
            _maxY = _minY;
            _minY = min;
            _dirY = -1;
        }
        setup++;
    }

    public void SetMinElbow(float angle)
    {
        _minY = angle;
        setup++;
    }

    /// <summary>
    /// Returns the percent value of the given angle inside the calibrated min and max angles.
    /// </summary>
    /// <param name="angle">The current angle</param>
    /// <returns>Percentage</returns>
    public float ElbowPercent (float angle)
    {
        var percent = Percent(angle, _minY, _maxY, _dirY);
        //var poly = Polynomial(percent);

        return percent;
    }

    /// <summary>
    /// Returns the percent value of the given angle inside the calibrated min and max angles.
    /// </summary>
    /// <param name="angle">The current angle</param>
    /// <returns>Percentage</returns>
    public float WristPercent(float angle)
    {
        var percent = Percent(angle, _minX, _maxX, _dirX);
        //var poly = Polynomial(percent);

        return percent;
    }

    /// <summary>
    /// USed to calculate the percent value of the given angle between the given min and max values.
    /// </summary>
    /// <param name="angle">Current angle</param>
    /// <param name="min">Min angle</param>
    /// <param name="max">Max angle</param>
    /// <returns>Percentage</returns>
    private float Percent(float angle, float min, float max, int dir)
    {
        angle = Mathf.Clamp(angle, min, max);
        var middle = (min + max) / 2f;
        var range = max - middle;
        var dist = angle - middle;
        var pct = dist / range;

        return pct * dir;
    }

    private float Polynomial(float pct)
    {
        var poly = (0.1f * Mathf.Pow(pct * 10, 2)) / 10;
        poly = (pct >= 0) ? poly : -poly;

        return poly;
    }

}
