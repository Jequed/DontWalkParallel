using UnityEngine;

public class TimerTrigger : Trigger
{
	[SerializeField]
	private float duration = 1.0f;

	private float startTime;

	protected virtual void Start()
	{
		startTime = Time.time;
	}

	protected virtual void Update()
	{
		if (Time.time - startTime > duration)
			ActivateTrigger();
	}
}