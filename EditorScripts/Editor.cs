using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class Editor : MonoBehaviour
{
    public enum EditingMode
    {
        level,
        map
    }

	[SerializeField]
	private GameController gameController;

	[SerializeField]
	private EditorLeftPanel leftPanel;

	[SerializeField]
	private EditorRightPanel rightPanel;

    [SerializeField]
    private Toggle logo;

	[SerializeField]
	private GameObject selector;

    [SerializeField]
    private GameObject dialogBackground;
    [SerializeField]
    private EditorDialogMessage dialogMessage;
    [SerializeField]
    private EditorDialogSave dialogSave;
    [SerializeField]
    private EditorDialogOpen dialogOpen;
    [SerializeField]
    private EditorDialogColorPicker dialogColorPicker;
    [SerializeField]
    private EditorDialogArtSettings dialogArtSettings;

    private LevelEditor levelEditor;
    private MapEditor mapEditor;

    private EditingMode editingMode = EditingMode.level;

	private Vector3 mouseWorldPosition;
	private Vector3 mouseGridPosition;
	private Vector3 previousMouseWorldPosition;
	private Vector3 mousePanPositionStart;
    private float mousePinchZoomStart;

    private bool deleteMode = false;

    private LineDrawer lineDrawer;

    private LineDrawer.LineCollection grid;

    private List<EditorDialog> dialogs;

    private const float minZoom = 0.2f;
    private const float maxZoom = 40.0f;
    private const float scrollWheelMultiplier = 1.0f;

    public LineDrawer LineDrawer
    {
        get
        {
            return lineDrawer;
        }
    }

    public MapContainer MapContainer
    {
        get
        {
            return gameController.MapContainer;
        }
    }
	public LevelContainer LevelContainer
	{
		get
		{
			return gameController.EditorLevelContainer;
		}
		set
		{
			gameController.EditorLevelContainer = value;
		}
	}

    public LevelEditor LevelEditor
    {
        get
        {
            return levelEditor;
        }
    }
    public MapEditor MapEditor
    {
        get
        {
            return mapEditor;
        }
    }

    public bool PlayMode
	{
		get
		{
			return GlobalData.playMode;
		}
		set
		{
			gameController.PlayMode = value;

			leftPanel.gameObject.SetActive(!value);
			rightPanel.gameObject.SetActive(!value);
			selector.gameObject.SetActive(!value);

            levelEditor.OnPlayModeChanged(value);
		}
	}

    public EditorLeftPanel LeftPanel
    {
        get
        {
            return leftPanel;
        }
    }
    public EditorRightPanel RightPanel
    {
        get
        {
            return rightPanel;
        }
    }

    public GameObject Selector
    {
        get
        {
            return selector;
        }
    }

	public Vector3 MouseWorldPosition
    {
        get
        {
            return mouseWorldPosition;
        }
    }
    public Vector3 PreviousMouseWorldPosition
    {
        get
        {
            return previousMouseWorldPosition;
        }
    }
    public Vector3 MouseGridPosition
    {
        get
        {
            return mouseGridPosition;
        }
    }

    public bool DeleteMode
    {
        get
        {
            return deleteMode;
        }
        set
        {
            deleteMode = value;
        }
    }

    void Awake()
	{
		lineDrawer = Camera.main.GetComponent<LineDrawer>();
        grid = lineDrawer.AddCollection(new LineDrawer.LineCollection(new Color(0.0f, 0.0f, 0.0f, 0.25f)));
        DrawGrid();

        mousePinchZoomStart = Camera.main.orthographicSize;

        levelEditor = new LevelEditor(this);
        mapEditor = new MapEditor(this);

        SetEditingMode(EditingMode.level);

        dialogs = new List<EditorDialog>();
        dialogs.Add(dialogMessage);
        dialogs.Add(dialogSave);
        dialogs.Add(dialogOpen);
        dialogs.Add(dialogColorPicker);
        dialogs.Add(dialogArtSettings);

        //LevelContainer.Background = "Dorm/DormRoom";
        //LevelContainer.Foreground = "Dorm/DormRoom_F";
    }

	void Update()
	{
        InputUtility.Update();

        #if UNITY_ANDROID
        if (InputUtility.LeftMouse() || InputUtility.DoubleTouch())
        #endif
        {
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(InputUtility.MousePosition); mouseWorldPosition.z = 0.0f;
            mouseGridPosition = new Vector3(Mathf.Round(mouseWorldPosition.x), Mathf.Round(mouseWorldPosition.y));
        }

        #if UNITY_ANDROID
            if (InputUtility.LeftMouseDown())
                previousMouseWorldPosition = mouseWorldPosition;
        #endif

        if (GlobalData.error != "")
            ShowDialogMessage("Error: " + GlobalData.error);

        if (!PlayMode)
		{
            if (editingMode == EditingMode.level)
            {
                levelEditor.Update();

                if (Input.GetKeyDown(KeyCode.F2))
                    PlayMode = true;
            }
            else if (editingMode == EditingMode.map)
            {
                mapEditor.Update();
            }
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				PlayMode = false;
		}

		if (!MouseInEditor())
		{
            var mousePanDelta = mousePanPositionStart - mouseWorldPosition;

            #if !UNITY_ANDROID
			    if (InputUtility.MiddleMouse() && mousePanDelta.magnitude < 5.0f)
				    Camera.main.transform.position += mousePanDelta;
			    else
				    mousePanPositionStart = mouseWorldPosition;

			    var mouseDelta = Input.mouseScrollDelta;
			    if (mouseDelta.magnitude > 0.1f)
				    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - mouseDelta.y * scrollWheelMultiplier, minZoom, maxZoom);
            #endif
            
            
            if (InputUtility.DoubleTouch())
            {
                if (mousePanDelta.magnitude < 5.0f)
				    Camera.main.transform.position += mousePanDelta;

                Camera.main.orthographicSize = Mathf.Clamp(mousePinchZoomStart / InputUtility.PinchDelta, minZoom, maxZoom);
            }
            else
            {
                #if UNITY_ANDROID
                    mousePanPositionStart = mouseWorldPosition;
                    mousePinchZoomStart = Camera.main.orthographicSize;
                #endif
            }
        }

        #if UNITY_ANDROID
        if (InputUtility.LeftMouse() || InputUtility.DoubleTouch())
        #endif
            previousMouseWorldPosition = mouseWorldPosition;
	}

    private void DrawGrid()
    {
        grid.Clear();

        for (float x = -15.5f; x < 15.5f; x += 1.0f)
        {
            grid.AddPoint(new Vector3(x, -100.0f, -20.0f));
            grid.AddPoint(new Vector3(x, 100.0f, -20.0f));
        }
        for (float y = -10.5f; y < 10.5f; y += 1.0f)
        {
            grid.AddPoint(new Vector3(-100.0f, y, -20.0f));
            grid.AddPoint(new Vector3(100.0f, y, -20.0f));
        }
    }
    
	public bool MouseInEditor()
	{
        Vector3 bottomLeft, topRight;
        bool inLeft = UIUtility.InImage(InputUtility.MousePosition, leftPanel.GetComponent<Image>(), out bottomLeft, out topRight);
        bool inRight = UIUtility.InImage(InputUtility.MousePosition, rightPanel.GetComponent<Image>(), out bottomLeft, out topRight);

        return inLeft || inRight || dialogBackground.gameObject.activeSelf;
	}

    public void SetEditingMode(EditingMode mode)
    {
        editingMode = mode;

        LevelContainer.gameObject.SetActive(mode == EditingMode.level);
        MapContainer.gameObject.SetActive(mode == EditingMode.map);

        leftPanel.LevelPanel.gameObject.SetActive(mode == EditingMode.level);
        leftPanel.MapPanel.gameObject.SetActive(mode == EditingMode.map);

        rightPanel.LevelPanel.gameObject.SetActive(mode == EditingMode.level);
        rightPanel.MapPanel.gameObject.SetActive(mode == EditingMode.map);

        grid.Visible = mode == EditingMode.level;
    }

    public void ShowDialogMessage(string message)
    {
        dialogMessage.Text.text = message;

        ShowDialog(dialogMessage);
    }
    public void ShowDialogSave()
    {
        ShowDialog(dialogSave);
    }
    public void ShowDialogOpen()
    {
        ShowDialog(dialogOpen);
    }
    public void ShowDialogColorPicker(bool isPrimary)
    {
        dialogColorPicker.IsPrimary = isPrimary;

        ShowDialog(dialogColorPicker);
    }
    public void ShowDialogArtSettings()
    {
        ShowDialog(dialogArtSettings);
    }

    public void ShowDialog(EditorDialog dialog)
    {
        dialog.Editor = this;

        dialogBackground.gameObject.SetActive(true);
        dialog.gameObject.SetActive(true);
        foreach (var d in dialogs)
        {
            if (d != dialog)
                d.gameObject.SetActive(false);
        }
    }
    public void HideDialog()
    {
        dialogBackground.gameObject.SetActive(false);
        foreach (var d in dialogs)
            d.gameObject.SetActive(false);
    }

	public void SaveLevel(string path)
	{
        /*#if UNITY_EDITOR

            var prefab = PrefabUtility.CreateEmptyPrefab(path);

            PrefabUtility.ReplacePrefab(LevelContainer.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

        #else*/

            LevelIO.SaveLevel(path, LevelContainer);

        //#endif
    }
	public void OpenLevel(string path)
	{
        levelEditor.OpenLevel(path, LevelContainer);
    }
}