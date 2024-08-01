using UnityEngine;

public class EditorView : MonoBehaviour
{
	protected Editor editor;

	protected virtual void Start()
	{
		editor = GetComponentInParent<Editor>();
	}
}