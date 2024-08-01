using UnityEngine;

public class LevelConnection : DWPObject
{
    [SerializeField]
    private SpriteRenderer lineSprite;

    [SerializeField]
    private SpriteRenderer arrowSprite;

    private MapLevel fromLevel;
    private MapLevel toLevel;

    private Vector3 fromLevelPoint;
    private Vector3 toLevelPoint;

    private const float zDepth = -5.0f;

    public MapLevel FromLevel
    {
        get
        {
            return fromLevel;
        }
        set
        {
            fromLevel = value;
        }
    }

    public MapLevel ToLevel
    {
        get
        {
            return toLevel;
        }
        set
        {
            toLevel = value;
        }
    }

    public float Angle
    {
        get
        {
            Vector3 direction = (toLevel.transform.position - fromLevel.transform.position).normalized;
            return MathUtility.GetAngleFromVector(new Vector2(-direction.x, -direction.y));
        }
    }
    public float Length
    {
        get
        {
            return Vector3.Distance(fromLevelPoint, toLevelPoint);
        }
    }

    public void UpdatePosition()
    {
        Vector3 direction = (toLevel.transform.position - fromLevel.transform.position).normalized;
        Vector3 directionNormal = new Vector3(-direction.y, direction.x);

        fromLevelPoint = fromLevel.transform.position + FindIntersectionInLevel(fromLevel, direction);
        toLevelPoint = toLevel.transform.position + FindIntersectionInLevel(toLevel, -direction);

        transform.position = (fromLevelPoint + toLevelPoint) * 0.5f + Vector3.forward * zDepth;
        
        lineSprite.transform.localScale = new Vector3(Vector3.Distance(toLevelPoint, fromLevelPoint), 1.0f, 1.0f);
        arrowSprite.transform.position = toLevelPoint + Vector3.forward * zDepth;

        var angle = MathUtility.GetAngleFromVector(new Vector2(-direction.x, -direction.y));
        lineSprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        arrowSprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    public Vector3 FindIntersectionInLevel(MapLevel level, Vector3 direction)
    {
        Vector3 intersection = Vector3.zero;

        if (RayToLineSegment(0.0f, 0.0f, direction.x, direction.y, toLevel.BorderScale.x * -0.5f, toLevel.BorderScale.y * -0.5f, toLevel.BorderScale.x * -0.5f, toLevel.BorderScale.y * 0.5f, out intersection))
            return intersection;
        if (RayToLineSegment(0.0f, 0.0f, direction.x, direction.y, toLevel.BorderScale.x * -0.5f, toLevel.BorderScale.y * 0.5f, toLevel.BorderScale.x * 0.5f, toLevel.BorderScale.y * 0.5f, out intersection))
            return intersection;
        if (RayToLineSegment(0.0f, 0.0f, direction.x, direction.y, toLevel.BorderScale.x * 0.5f, toLevel.BorderScale.y * 0.5f, toLevel.BorderScale.x * 0.5f, toLevel.BorderScale.y * -0.5f, out intersection))
            return intersection;
        if (RayToLineSegment(0.0f, 0.0f, direction.x, direction.y, toLevel.BorderScale.x * 0.5f, toLevel.BorderScale.y * -0.5f, toLevel.BorderScale.x * -0.5f, toLevel.BorderScale.y * -0.5f, out intersection))
            return intersection;

        return intersection;
    }

    public static bool RayToLineSegment(float x, float y, float dx, float dy, float x1, float y1, float x2, float y2, out Vector3 result)
    {
        float r, s, d;

        if (dy / dx != (y2 - y1) / (x2 - x1))
        {
            d = ((dx * (y2 - y1)) - dy * (x2 - x1));
            if (d != 0)
            {
                r = (((y - y1) * (x2 - x1)) - (x - x1) * (y2 - y1)) / d;
                s = (((y - y1) * dx) - (x - x1) * dy) / d;
                if (r >= 0 && s >= 0 && s <= 1)
                {
                    result = new Vector3(x + r * dx, y + r * dy);
                    return true;
                }
            }
        }

        result = Vector3.zero;
        return false;
    }
}