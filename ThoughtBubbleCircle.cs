using UnityEngine;

public class ThoughtBubbleCircle : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer whiteCircle;

    [SerializeField]
    private SpriteRenderer blackCircle;

    private float minRadius;
    private float maxRadius;

    private float duration;

    private float delay;

    private const float minDuration = 1.3f;
    private const float maxDuration = 1.8f;

    private const float minDelay = -0.5f;
    private const float maxDelay = 0.5f;

    public float MinRadius
    {
        get
        {
            return minRadius;
        }
    }

    public float MaxRadius
    {
        get
        {
            return maxRadius;
        }
    }

    public void Initialize(float minRadius, float maxRadius)
    {
        this.minRadius = minRadius;
        this.maxRadius = maxRadius;

        duration = MathUtility.GetRandomRange(minDuration, maxDuration);

        delay = MathUtility.GetRandomRange(minDelay, maxDelay);

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, MathUtility.GetRandomRange(0.0f, 360.0f));

        UpdateSize();
    }

    private void Update()
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        float ratio = (Mathf.Sin((Time.time + delay) / duration * Mathf.PI * 2.0f) + 1.0f) * 0.5f;

        float radius = minRadius + (maxRadius - minRadius) * ratio;

        float whiteCircleScale = radius * 2.0f;
        float blackCircleScale = whiteCircleScale + 0.1f;

        whiteCircle.transform.localScale = new Vector3(whiteCircleScale, whiteCircleScale, 1.0f);
        blackCircle.transform.localScale = new Vector3(blackCircleScale, blackCircleScale, 1.0f);
    }
}