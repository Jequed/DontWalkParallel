using System.Collections;
using UnityEngine;

public class FitToScreenWidth : MonoBehaviour
{
	private Canvas canvas;
	private RectTransform canvasRectTransform;
	private RectTransform rectTransform;

	private int cachedScreenWidth = 0;
	private int cachedScreenHeight = 0;

	private float screenWidthInUISpace = 0f;

	public void Start()
	{
		if (canvas == null)
		{
			canvas = GetComponentInParent<Canvas>();
			canvasRectTransform = canvas.GetComponent<RectTransform>();
			rectTransform = GetComponent<RectTransform>();
		}
	}

	public void Update()
	{
		if (cachedScreenWidth != Screen.width || cachedScreenHeight != Screen.height)
		{
			StartCoroutine(SetWidth());
		}
	}
	private IEnumerator SetWidth()
	{
		cachedScreenWidth = Screen.width;
		cachedScreenHeight = Screen.height;

		// Sometimes it will take the UI a full frame to settle down, so we delay calculation to wait for this to occur.
		yield return 0;

		Vector3 bottomLeftWorld;
		Vector3 topRightWorld;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, new Vector2(0, 0), canvas.worldCamera, out bottomLeftWorld);
		RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, new Vector2(cachedScreenWidth, cachedScreenHeight), canvas.worldCamera, out topRightWorld);
		screenWidthInUISpace = topRightWorld.x - bottomLeftWorld.x;

		rectTransform.sizeDelta = new Vector2(screenWidthInUISpace, rectTransform.sizeDelta.y);
	}
}