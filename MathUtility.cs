using UnityEngine;

public static class MathUtility
{
    public static float GetAngleFromVector(Vector2 vector)
    {
        float angle = Vector2.Angle(Vector2.down, vector);
        if (vector.x < 0.0f)
            angle *= -1.0f;

        angle += 90.0f;
        if (angle < 0.0f)
            angle += 360.0f;

        return angle;
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector3(-Mathf.Cos(radians), -Mathf.Sin(radians));
    }

    public static float GetRandomRange(float min, float max)
    {
        return min + (float)GlobalData.random.NextDouble() * (max - min);
    }
}