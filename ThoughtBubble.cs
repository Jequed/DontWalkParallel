using System.Linq;
using UnityEngine;

public class ThoughtBubble : DWPObject
{
    [SerializeField]
    private SpriteRenderer mainCircle;

    [SerializeField]
    private TextMesh text;

    [SerializeField]
    private GameObject graphicsContainer;

    [SerializeField]
    private GameObject tailContainer;

    [SerializeField]
    private BoxCollider thoughtBubbleIcon;

    private ThoughtBubbleCircle tailCircle1;
    private ThoughtBubbleCircle tailCircle2;
    private ThoughtBubbleCircle tailCircle3;

    private bool isTweening = false;
    private bool isGrowing = false;
    private float tweenStartTime;

    private Vector3 originalPosition;

    private const float minCircleMedian = 0.4f;
    private const float minCircleRange = 0.15f;
    private const float maxCircleMedian = 0.8f;
    private const float maxCircleRange = 0.15f;

    private const float minCircleRadius1 = minCircleMedian - minCircleRange;
    private const float minCircleRadius2 = minCircleMedian + minCircleRange;
    private const float maxCircleRadius1 = maxCircleMedian - maxCircleRange;
    private const float maxCircleRadius2 = maxCircleMedian + maxCircleRange;

    private const float padding = 0.1f;

    private const float tweenDuration = 1.0f;

    private bool IsShowing
    {
        get
        {
            return graphicsContainer.gameObject.activeSelf && tailContainer.activeSelf;
        }
        set
        {
            graphicsContainer.gameObject.SetActive(value);
            tailContainer.gameObject.SetActive(value);
        }
    }

    public string Text
    {
        get
        {
            return text.text;
        }
        set
        {
            text.text = value;
        }
    }

    protected override void GameStart()
    {
        base.GameStart();

        originalPosition = transform.position;

        isTweening = false;

        if (text.text != "")
        {
            IsShowing = true;

            int lineCount = Text.Select((c, i) => Text.Substring(i)).Count(sub => sub.StartsWith("\n")) + 1;

            Initialize(8.0f, lineCount);
        }
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        if (text.text != "")
            UpdateTail();

        ProcessTween();

#if !UNITY_ANDROID
        if (Input.GetMouseButton(0))
            ProcessTouch(Input.mousePosition);
#else
		foreach (Touch touch in Input.touches)
			{
				ProcessTouch(touch.position);
			}
#endif
    }

    private void Initialize(float width, float height)
    {
        mainCircle.transform.localScale = new Vector3(width, height, 1.0f);

        const float delta = 0.001f;

        var firstCircle = CreateCircle(new Vector3(width * 0.5f, 0.0f, 0.0f), MathUtility.GetRandomRange(minCircleRadius1, minCircleRadius2), MathUtility.GetRandomRange(maxCircleRadius1, maxCircleRadius2), graphicsContainer.transform);
        var lastCircle = firstCircle;

        CreateCircle(new Vector3(-width * 0.5f, 0.0f, 0.0f), MathUtility.GetRandomRange(minCircleRadius1, minCircleRadius2), MathUtility.GetRandomRange(maxCircleRadius1, maxCircleRadius2), graphicsContainer.transform);

        var nextMinRadius = MathUtility.GetRandomRange(minCircleRadius1, minCircleRadius2);

        for (float angle = 0.0f; angle < Mathf.PI * 2.0f; angle += delta)
        {
            var position = new Vector3(Mathf.Cos(angle) * width * 0.5f, Mathf.Sin(angle) * height * 0.5f, 0.0f);

            if (Vector3.Distance(position, lastCircle.transform.localPosition) >= (lastCircle.MinRadius + nextMinRadius - padding) / transform.lossyScale.x)
                lastCircle = CreateCircle(position, nextMinRadius, MathUtility.GetRandomRange(maxCircleRadius1, maxCircleRadius2), graphicsContainer.transform);
        }

        if (Vector3.Distance(lastCircle.transform.localPosition, firstCircle.transform.localPosition) > minCircleRadius1 * 2.0f - padding)
            CreateCircle((lastCircle.transform.localPosition + firstCircle.transform.localPosition) * 0.5f, MathUtility.GetRandomRange(minCircleRadius1, minCircleRadius2), MathUtility.GetRandomRange(maxCircleRadius1, maxCircleRadius2), graphicsContainer.transform);

        tailCircle1 = CreateCircle(Vector3.zero, 0.05f, 0.15f, tailContainer.transform);
        tailCircle2 = CreateCircle(Vector3.zero, 0.15f, 0.25f, tailContainer.transform);
        tailCircle3 = CreateCircle(Vector3.zero, 0.25f, 0.35f, tailContainer.transform);

        UpdateTail();
    }

    private void UpdateTail()
    {
        float z = tailCircle1.transform.position.z;

        var ourPosition = originalPosition;
        var playerPosition = GlobalData.player.transform.position + Vector3.up * 0.5f;

        var c1 = playerPosition + (ourPosition - playerPosition) * 0.1f;
        var c2 = playerPosition + (ourPosition - playerPosition) * 0.35f;
        var c3 = playerPosition + (ourPosition - playerPosition) * 0.6f;

        tailCircle1.transform.position = new Vector3(c1.x, c1.y, z);
        tailCircle2.transform.position = new Vector3(c2.x, c2.y, z);
        tailCircle3.transform.position = new Vector3(c3.x, c3.y, z);
    }

    private ThoughtBubbleCircle CreateCircle(Vector3 position, float minRadius, float maxRadius, Transform parent)
    {
        var circle = Instantiate(Resources.Load<ThoughtBubbleCircle>("Prefabs/GamePrefabs/ThoughtBubbleCircle"));
        circle.Initialize(minRadius, maxRadius);

        circle.transform.parent = parent;
        circle.transform.localPosition = position;

        return circle;
    }

    private void ProcessTween()
    {
        if (isTweening)
        {
            float ratio = (Time.time - tweenStartTime) / tweenDuration;
            if (!isGrowing)
                ratio = 1.0f - ratio;

            if (ratio >= 0.0f && ratio <= 1.0f)
            {
                graphicsContainer.transform.position = thoughtBubbleIcon.transform.position + (originalPosition - thoughtBubbleIcon.transform.position) * ratio;

                graphicsContainer.transform.localScale = Vector3.one * ratio;

                tailCircle1.gameObject.SetActive(ratio > 0.2f);
                tailCircle2.gameObject.SetActive(ratio > 0.5f);
                tailCircle3.gameObject.SetActive(ratio > 0.8f);
            }
            else
            {
                IsShowing = isGrowing;
                isTweening = false;

                tailCircle1.gameObject.SetActive(isGrowing);
                tailCircle2.gameObject.SetActive(isGrowing);
                tailCircle3.gameObject.SetActive(isGrowing);
            }
        }
    }

    private void ProcessTouch(Vector3 touchPosition)
    {
        if (IsShowing && !isTweening)
        {
            isTweening = true;
            isGrowing = false;
            tweenStartTime = Time.time;
        }

        if (!IsShowing)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == thoughtBubbleIcon && Text != "")
                {
                    isTweening = true;
                    IsShowing = true;
                    isGrowing = true;
                    tweenStartTime = Time.time;
                }
            }
        }
    }
}