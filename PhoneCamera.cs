using UnityEngine;

public class PhoneCamera : MonoBehaviour
{
	private new Camera camera;

	void Start()
	{
		camera = GetComponent<Camera>();

		camera.aspect = GlobalData.phoneAspectRatio;
	}

	void Update()
	{
		camera.orthographicSize = 3.59f * transform.lossyScale.x;
	}
}