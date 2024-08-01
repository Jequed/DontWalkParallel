using UnityEngine;

public class WorldObject : DWPObject
{
	[SerializeField]
	protected SpriteRenderer mainSpriteRenderer;

	private const float defaultAnimationSpeed = 5.0f;

	private ImageSeries currentImageSeries = null;
	private float playSpeedMultiplier = 1.0f;
	private float currentImageSeriesStartTime = 0.0f;

	private bool flashing = false;

	public bool Flashing
	{
		get
		{
			return flashing;
		}
		set
		{
			if (value != flashing)
			{
				flashing = value;

				if (!value && mainSpriteRenderer != null)
					mainSpriteRenderer.color = Color.white;

				if (value)
					OnFlashing();
			}
		}
	}

    public ImageSeries CurrentImageSeries
    {
        get
        {
            return currentImageSeries;
        }
    }

    public float PlaySpeedMultiplier
    {
        get
        {
            return playSpeedMultiplier;
        }
        set
        {
            playSpeedMultiplier = value;
        }
    }

    public SpriteRenderer MainSpriteRenderer
    {
        get
        {
            return mainSpriteRenderer;
        }
    }

	protected override void Start()
	{
		base.Start();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (mainSpriteRenderer != null)
		{
            if (currentImageSeries != null)
                UpdateSprite(mainSpriteRenderer, currentImageSeries);

			if (flashing)
				OnFlashing();
		}
	}

    public void UpdateSprite(SpriteRenderer spriteRenderer, ImageSeries imageSeries)
    {
        if (imageSeries != null)
        {
            if (spriteRenderer.flipX != imageSeries.flipX)
                spriteRenderer.flipX = imageSeries.flipX;
            if (spriteRenderer.flipY != imageSeries.flipY)
                spriteRenderer.flipY = imageSeries.flipY;

            if (GlobalData.playMode)
                spriteRenderer.sprite = imageSeries.sprites[(int)Mathf.Floor(Mathf.Repeat((Time.time - currentImageSeriesStartTime) * imageSeries.speed * defaultAnimationSpeed * playSpeedMultiplier * GlobalData.timeMultiplier, imageSeries.sprites.Length))];
            else
                spriteRenderer.sprite = imageSeries.sprites[0];
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }

    public void SetImageSeries(ImageSeries imageSeries, float playSpeedMultiplier = 1.0f)
	{
        if (imageSeries != null && imageSeries != currentImageSeries)
        {
            currentImageSeries = imageSeries;
            this.playSpeedMultiplier = playSpeedMultiplier;
        }
	}

	private void OnFlashing()
	{
		if (mainSpriteRenderer != null)
			mainSpriteRenderer.color = Color.Lerp(Color.black, Color.red, 0.75f + Mathf.Sin(Time.time * 30.0f) * 0.25f);
	}

	public static Vector3 SnapPointToGrid(Vector3 point)
	{
		return new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));
	}

    public void RandomizeAnimationFrame(float spread = 1.0f)
    {
        currentImageSeriesStartTime = (float)GlobalData.random.NextDouble() * spread;
    }
}