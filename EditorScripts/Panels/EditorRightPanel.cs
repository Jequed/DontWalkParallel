using UnityEngine;

public class EditorRightPanel : MonoBehaviour
{
    [SerializeField]
    private EditorRightPanelLevelEditor levelPanel;

    [SerializeField]
    private EditorRightPanelMapEditor mapPanel;

    public EditorRightPanelLevelEditor LevelPanel
    {
        get
        {
            return levelPanel;
        }
    }
    public EditorRightPanelMapEditor MapPanel
    {
        get
        {
            return mapPanel;
        }
    }
}