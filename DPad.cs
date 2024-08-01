using UnityEngine;

public class DPad : MonoBehaviour
{
	[SerializeField]
	private Collider colliderLeft;

	[SerializeField]
	private Collider colliderRight;

	[SerializeField]
	private Collider colliderDown;

	[SerializeField]
	private Collider colliderUp;

	protected virtual void Update()
	{
		if (GlobalData.playMode)
			GetInput();
	}

	private void GetInput()
	{
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

	private void ProcessTouch(Vector3 touchPosition)
	{
		if (GlobalData.player != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider == colliderLeft)
					GlobalData.player.OnPressLeft();
				else if (hit.collider == colliderRight)
					GlobalData.player.OnPressRight();
				else if (hit.collider == colliderDown)
					GlobalData.player.OnPressDown();
				else if (hit.collider == colliderUp)
					GlobalData.player.OnPressUp();
			}
		}
	}
}