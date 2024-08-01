using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToCamera : MonoBehaviour
{
	private int cachedScreenWidth = 0;
	private int cachedScreenHeight = 0;
	private Vector3 cachedCameraPosition = Vector3.zero;
	private float cachedCameraSize = 0.0f;

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (cachedScreenWidth != Screen.width || cachedScreenHeight != Screen.height ||
			(cachedCameraPosition - Camera.main.transform.position).magnitude > 0.0001f ||
			Mathf.Abs(cachedCameraSize - Camera.main.orthographicSize) > 0.0001f)
		{
			Resize();
		}
	}
	private void Resize()
	{
		cachedScreenWidth = Screen.width;
		cachedScreenHeight = Screen.height;
		cachedCameraPosition = Camera.main.transform.position;
		cachedCameraSize = Camera.main.orthographicSize;

		Vector3 localScale = Vector3.one;

		var width = spriteRenderer.sprite.bounds.size.x;
		var height = spriteRenderer.sprite.bounds.size.y;

		var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		localScale.x = worldScreenWidth / width;
		localScale.y = worldScreenHeight / height;

		transform.localScale = localScale;

		transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
	}
}