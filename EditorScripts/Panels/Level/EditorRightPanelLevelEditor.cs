using UnityEngine;

public class EditorRightPanelLevelEditor : EditorView
{
    [SerializeField]
    private GameObject levelPanel;

    [SerializeField]
    private GameObject blockPanel;

    [SerializeField]
    private GameObject peoplePanel;

    [SerializeField]
    private GameObject miscPanel;

    [SerializeField]
    private GameObject artPanel;

    public LevelContainer.LayerType LayerMode
    {
        set
        {
            levelPanel.SetActive(value == LevelContainer.LayerType.level);
            blockPanel.SetActive(value == LevelContainer.LayerType.blocks);
            peoplePanel.SetActive(value == LevelContainer.LayerType.people);
            miscPanel.SetActive(value == LevelContainer.LayerType.misc);
            artPanel.SetActive(value == LevelContainer.LayerType.art);
        }
    }
}