using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	[SerializeField]
	private string sceneName;

	protected virtual void OnTriggerEnter()
	{
		SceneManager.LoadScene("Resources/Scenes/Levels/" + sceneName);
	}
}