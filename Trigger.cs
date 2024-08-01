using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
	[SerializeField]
	private NPCActionController actionSeries;

	private bool triggered = false;

	public void ActivateTrigger()
	{
		if (!triggered)
		{
			triggered = true;
			actionSeries.BeginActions();
		}
	}
}