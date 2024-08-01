using UnityEngine;
using UnityEngine.UI;

public class EditorLeftPanel : EditorView
{
    [SerializeField]
    EditorLeftPanelLevelEditor levelPanel;

    [SerializeField]
    EditorLeftPanelMapEditor mapPanel;

    [SerializeField]
    private Toggle mapToggle;

    [SerializeField]
    private Toggle logoToggle;

    public EditorLeftPanelLevelEditor LevelPanel
    {
        get
        {
            return levelPanel;
        }
    }
    public EditorLeftPanelMapEditor MapPanel
    {
        get
        {
            return mapPanel;
        }
    }

    protected override void Start()
    {
        base.Start();

        mapToggle.onValueChanged.AddListener(MapToggle_OnValueChanged);
        logoToggle.onValueChanged.AddListener(LogoToggle_OnValueChanged);
    }

    void OnDestroy()
    {
        mapToggle.onValueChanged.RemoveListener(MapToggle_OnValueChanged);
        logoToggle.onValueChanged.RemoveListener(LogoToggle_OnValueChanged);
    }

    private void MapToggle_OnValueChanged(bool value)
    {
        editor.SetEditingMode(value ? Editor.EditingMode.map : Editor.EditingMode.level);
    }

    private void LogoToggle_OnValueChanged(bool value)
    {
        editor.DeleteMode = value;
    }
}