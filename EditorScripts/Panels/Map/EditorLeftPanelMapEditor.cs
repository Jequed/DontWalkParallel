using UnityEngine;
using UnityEngine.UI;

public class EditorLeftPanelMapEditor : EditorView
{
    [SerializeField]
    private Button levelButton;

    [SerializeField]
    private Button connectionsButton;

    [SerializeField]
    private Button canvasButton;

    [SerializeField]
    private Button saveButton;

    protected override void Start()
	{
		base.Start();

        levelButton.onClick.AddListener(levelButton_OnClick);
        connectionsButton.onClick.AddListener(connectionsButton_OnClick);
        canvasButton.onClick.AddListener(canvasButton_OnClick);
        saveButton.onClick.AddListener(SaveButton_OnClick);

        UpdateLayerButtons();
    }

	void OnDestroy()
	{
        levelButton.onClick.RemoveListener(levelButton_OnClick);
        connectionsButton.onClick.RemoveListener(connectionsButton_OnClick);
        canvasButton.onClick.RemoveListener(canvasButton_OnClick);
        saveButton.onClick.RemoveListener(SaveButton_OnClick);
    }

    private void levelButton_OnClick()
    {
        editor.MapEditor.EditType = MapEditor.EditingType.level;
        UpdateLayerButtons();
    }
    private void connectionsButton_OnClick()
    {
        editor.MapEditor.EditType = MapEditor.EditingType.connections;
        UpdateLayerButtons();
    }
    private void canvasButton_OnClick()
    {
        editor.MapEditor.EditType = MapEditor.EditingType.canvas;
        UpdateLayerButtons();
    }
    private void UpdateLayerButtons()
    {
        levelButton.interactable = (editor.MapEditor.EditType != MapEditor.EditingType.level);
        connectionsButton.interactable = (editor.MapEditor.EditType != MapEditor.EditingType.connections);
        canvasButton.interactable = (editor.MapEditor.EditType != MapEditor.EditingType.canvas);
    }

    private void SaveButton_OnClick()
    {
        #if UNITY_ANDROID
            MapIO.SaveMap(Application.persistentDataPath + "/map", editor.MapContainer);
        #else
            MapIO.SaveMap(Application.dataPath + "/map", editor.MapContainer);
        #endif
    }
}