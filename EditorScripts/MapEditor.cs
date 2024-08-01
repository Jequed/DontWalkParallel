using System;
using System.IO;
using UnityEngine;

public class MapEditor
{
    public event Action<DWPObject, DWPObject> OnSelectionChanged;

    public enum EditingType
    {
        level,
        connections,
        canvas
    }

    private Editor editor;

    private DWPObject selectedItem = null;

    private bool dragItem = false;
    private bool isInitialClick = false;
    private Vector3 selectedLevelStartPosition = Vector3.zero;
    private Vector3 selectedMouseStartPosition = Vector3.zero;

    private MapLevel connectionFromLevel = null;
    private bool isMakingConnection = false;

    private LineDrawer.LineCollection greenLines;

    private EditingType editType;

    private readonly Color canvasPaintColor = Color.black;
    private readonly Color canvasClearColor = new Color32(138, 165, 112, 255);

    public EditingType EditType
    {
        get
        {
            return editType;
        }
        set
        {
            editType = value;

            editor.RightPanel.MapPanel.EditType = value;

            isMakingConnection = false;
        }
    }

    public DWPObject SelectedItem
    {
        get
        {
            return selectedItem;
        }
        set
        {
            if (value != selectedItem)
            {
                var previousItem = selectedItem;
                selectedItem = value;
                if (OnSelectionChanged != null)
                    OnSelectionChanged(previousItem, selectedItem);
            }
        }
    }

    public MapEditor(Editor editor)
    {
        this.editor = editor;

        greenLines = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(Color.green));

        EditType = EditingType.level;

        #if UNITY_ANDROID
            MapIO.OpenMap(Application.persistentDataPath + "/map", editor.MapContainer);
        #else
            MapIO.OpenMap(Application.dataPath + "/map", editor.MapContainer);
        #endif
    }

    public void Update()
    {
        switch (editType)
        {
            default:
            case EditingType.level:
                LevelPlacementControl();
                break;
            case EditingType.connections:
                ConnectionsControl();
                break;
            case EditingType.canvas:
                CanvasControl();
                break;
        }

        editor.Selector.SetActive(SelectedItem);
        if (SelectedItem)
        {
            editor.Selector.transform.position = new Vector3(SelectedItem.transform.position.x, SelectedItem.transform.position.y, 0.0f);
            editor.Selector.transform.localRotation = Quaternion.identity;

            if (SelectedItem is MapLevel)
            {
                var mapLevel = SelectedItem as MapLevel;

                editor.Selector.transform.localScale = mapLevel.Scale * 1.5f;
            }
            else if (SelectedItem is LevelConnection)
            {
                var connection = SelectedItem as LevelConnection;

                editor.Selector.transform.localScale = new Vector3(connection.Length, 0.4f, 1.0f);
                editor.Selector.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, connection.Angle);
            }
        }

        UpdateConnections();

        greenLines.Clear();
        if (isMakingConnection)
        {
            greenLines.AddPoint(connectionFromLevel.transform.position);
            greenLines.AddPoint(editor.MouseWorldPosition);
        }
    }

    private void LevelPlacementControl()
    {
        MapLevel hoverLevel = null;
        if (InputUtility.LeftMouse())
        {
            var ray = Camera.main.ScreenPointToRay(InputUtility.MousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

            foreach (var hit in hits)
            {
                var level = hit.transform.GetComponentInParent<MapLevel>();

                if (level != null)
                    hoverLevel = level;
            }
        }

        if (InputUtility.LeftMouse())
        {
            if (!editor.MouseInEditor())
            {
                if (!dragItem)
                {
                    if (hoverLevel != null)
                    {
                        SelectedItem = hoverLevel;
                        selectedLevelStartPosition = SelectedItem.transform.position;
                        selectedMouseStartPosition = editor.MouseWorldPosition;

                        if (!isInitialClick)
                            dragItem = true;
                    }
                }
                else
                {
                    if (SelectedItem != null)
                    {
                        var newLocalPosition = selectedLevelStartPosition + editor.MouseWorldPosition - selectedMouseStartPosition;
                        newLocalPosition.z = 0.0f;
                        SelectedItem.transform.localPosition = newLocalPosition;
                    }
                    else
                    {
                        dragItem = false;
                    }
                }
            }

            isInitialClick = true;
        }

        if (InputUtility.RightMouse())
            SelectedItem = null;

        if (!InputUtility.LeftMouse())
        {
            dragItem = false;
            isInitialClick = false;
        }
    }

    private void ConnectionsControl()
    {
        if (InputUtility.LeftMouseDown() && !editor.DeleteMode)
        {
            var ray = Camera.main.ScreenPointToRay(InputUtility.MousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

            foreach (var hit in hits)
            {
                var connection = hit.transform.GetComponentInParent<LevelConnection>();
                if (connection != null)
                {
                    SelectedItem = connection;
                    break;
                }

                var level = hit.transform.GetComponentInParent<MapLevel>();
                if (level != null)
                {
                    connectionFromLevel = level;
                    isMakingConnection = true;
                    break;
                }
            }
        }

        if (isMakingConnection)
        {
            if (!InputUtility.LeftMouse())
            {
                var ray = Camera.main.ScreenPointToRay(InputUtility.MousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

                foreach (var hit in hits)
                {
                    var level = hit.transform.GetComponentInParent<MapLevel>();
                    if (level != null && level != connectionFromLevel)
                    {
                        editor.MapContainer.AddConnection(connectionFromLevel, level);
                        break;
                    }
                }

                isMakingConnection = false;
            }
        }

        if (InputUtility.RightMouse() || (InputUtility.LeftMouse() && editor.DeleteMode))
        {
            SelectedItem = null;

            var ray = Camera.main.ScreenPointToRay(InputUtility.MousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

            foreach (var hit in hits)
            {
                var connection = hit.transform.GetComponentInParent<LevelConnection>();
                if (connection != null)
                {
                    editor.MapContainer.Connections.Remove(connection);
                    GameObject.Destroy(connection.gameObject);
                    break;
                }
            }
        }
    }

    private void CanvasControl()
    {
        if (InputUtility.LeftMouse())
            editor.MapContainer.Canvas.DrawLine(editor.PreviousMouseWorldPosition, editor.MouseWorldPosition, canvasPaintColor, 1);

        if (InputUtility.RightMouse() || (InputUtility.LeftMouse() && editor.DeleteMode))
            editor.MapContainer.Canvas.DrawLine(editor.PreviousMouseWorldPosition, editor.MouseWorldPosition, canvasClearColor, 10);
    }

    private void UpdateConnections()
    {
        var connections = editor.MapContainer.Connections;

        foreach (var connection in connections)
            connection.UpdatePosition();
    }
}