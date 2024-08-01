using UnityEngine;

public class PlayModeSwitch : MonoBehaviour
{
	[SerializeField]
	private GameObject playModeGameObject;

	[SerializeField]
	private GameObject editorGameObject;

	[SerializeField]
	private bool affectedByDebugMode = false;

	void Awake()
	{
		if (affectedByDebugMode)
		{
			if (playModeGameObject != null)
				playModeGameObject.SetActive(GlobalData.playMode && !GlobalData.debugMode);
			if (editorGameObject != null)
				editorGameObject.SetActive(!GlobalData.playMode || GlobalData.debugMode);
		}
		else
		{
			if (playModeGameObject != null)
				playModeGameObject.SetActive(GlobalData.playMode);
			if (editorGameObject != null)
				editorGameObject.SetActive(!GlobalData.playMode);
		}
	}
}