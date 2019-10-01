using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static class RotationDir
    {
        public const float CounterClockwise = 1.0f;
        public const float Clockwise = -1.0f;
    }
    public static Vector2 rotateVector(Vector2 v, float angleRad)
    {
        float _x = v.x * Mathf.Cos(angleRad) - v.y * Mathf.Sin(angleRad);
        float _y = v.x * Mathf.Sin(angleRad) + v.y * Mathf.Cos(angleRad);
        return new Vector2(_x, _y);
    }

}
