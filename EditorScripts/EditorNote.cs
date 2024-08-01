using UnityEngine;

public class EditorNote : DWPObject
{
    [SerializeField]
    private TextMesh textMesh;

    public string Text
    {
        get
        {
            return textMesh.text;
        }
        set
        {
            textMesh.text = value;
        }
    }
}