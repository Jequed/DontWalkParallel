using UnityEngine;

public class EditorRightPanelMapEditor : EditorView
{
    [SerializeField]
    private GameObject levelPanel;

    public MapEditor.EditingType EditType
    {
        set
        {
            levelPanel.SetActive(value == MapEditor.EditingType.level);
        }
    }
}