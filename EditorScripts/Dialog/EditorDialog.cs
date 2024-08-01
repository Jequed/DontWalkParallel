using UnityEngine;

public class EditorDialog : MonoBehaviour
{
    protected Editor editor;

    public Editor Editor
    {
        get
        {
            return editor;
        }
        set
        {
            editor = value;
        }
    }
}