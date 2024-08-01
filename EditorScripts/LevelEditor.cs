using System;
using UnityEngine;

public class LevelEditor
{
    public event Action<DWPObject, DWPObject> OnSelectionChanged;

    public event Action<DWPObject> OnSelectionMoved;

    public event Action OnOpenLevel;

    public enum LevelTranslationMode
    {
        none,
        //background,
        phone
    }

    private Editor editor;

    private DWPObject selectedItem;

    private DWPObject copiedItem = null;

    private bool dragItem = false;
    private bool isInitialClick = false;

    private ItemInfo.Type currentItemType = ItemInfo.Type.None;

    private ItemInfo.Type currentMiscItemType = ItemInfo.Type.Cloner;

    private LevelContainer.LayerType layerMode;

    private LevelTranslationMode leveltranslationMode = LevelTranslationMode.none;

    private LineDrawer.LineCollection peoplePaths;
    private LineDrawer.LineCollection selectedPersonPath;

    private LevelCanvasEditor canvasEditor;

    private bool defaultSeeOverBlocks = false;

    public ItemInfo.Type CurrentItemType
    {
        get
        {
            return currentItemType;
        }
        set
        {
            var info = GlobalData.GetInfo(value);
            if (info.Layer == LevelContainer.LayerType.misc)
                currentMiscItemType = value;

            currentItemType = value;
        }
    }

    public LevelContainer.LayerType EditingLayer
    {
        get
        {
            return layerMode;
        }
        set
        {
            layerMode = value;
            editor.RightPanel.LevelPanel.LayerMode = value;

            switch (value)
            {
                case LevelContainer.LayerType.blocks:
                    currentItemType = ItemInfo.Type.Block;
                    break;
                case LevelContainer.LayerType.people:
                    currentItemType = ItemInfo.Type.NPC;
                    break;
                case LevelContainer.LayerType.misc:
                    currentItemType = currentMiscItemType;
                    break;
                default:
                    currentItemType = ItemInfo.Type.None;
                    break;
            }
        }
    }

    public LevelTranslationMode LevelTranslateMode
    {
        get
        {
            return leveltranslationMode;
        }
        set
        {
            leveltranslationMode = value;
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

                UpdatePeoplePaths();
            }
        }
    }

    public bool DefaultSeeOverBlocks
    {
        get
        {
            return defaultSeeOverBlocks;
        }
        set
        {
            defaultSeeOverBlocks = value;
        }
    }

    public LevelCanvasEditor CanvasEditor
    {
        get
        {
            return canvasEditor;
        }
    }

    public LevelEditor(Editor editor)
    {
        this.editor = editor;

        EditingLayer = LevelContainer.LayerType.level;

        peoplePaths = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(new Color(1.0f, 0.0f, 0.0f, 1.0f)));
        selectedPersonPath = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(new Color(0.0f, 0.0f, 1.0f, 1.0f)));

        canvasEditor = new LevelCanvasEditor(editor);
    }

    public void Update()
    {
        if (!InputUtility.DoubleTouch())
        {
            switch (layerMode)
            {
                case LevelContainer.LayerType.level:
                    LevelOptionsControl();
                    break;
                case LevelContainer.LayerType.blocks:
                case LevelContainer.LayerType.people:
                case LevelContainer.LayerType.misc:
                    ItemPlacementControl();
                    break;
                case LevelContainer.LayerType.art:
                    CanvasControl();
                    break;
                default:
                    break;
            }
        }

		editor.Selector.SetActive(selectedItem);
        if (selectedItem)
        {
            editor.Selector.transform.position = selectedItem.transform.position;
            editor.Selector.transform.localScale = Vector3.one;
            editor.Selector.transform.localRotation = Quaternion.identity;
        }
    }

    private void ItemPlacementControl()
    {
        DWPObject hoverItem = null;
        if (InputUtility.LeftMouse() || InputUtility.RightMouse())
        {
            var ray = Camera.main.ScreenPointToRay(InputUtility.MousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

            foreach (var hit in hits)
            {
                var item = hit.transform.GetComponentInParent<DWPObject>();

                if (item != null && item.Info.Layer == layerMode)
                    hoverItem = item;
            }
        }

        if (InputUtility.LeftMouse() && !editor.DeleteMode)
        {
            if (!editor.MouseInEditor())
            {
                if (!dragItem)
                {
                    if (hoverItem == null)
                    {
                        if (currentItemType != ItemInfo.Type.None)
                        {
                            var item = GameObject.Instantiate(Resources.Load<DWPObject>(GlobalData.GetInfo(currentItemType).PrefabName));

                            GameObject container = null;
                            switch (item.Layer)
                            {
                                case LevelContainer.LayerType.blocks:
                                    container = editor.LevelContainer.BlocksContainer;
                                    (item as Block).CanSeeOver = defaultSeeOverBlocks;
                                    break;
                                case LevelContainer.LayerType.people:
                                    container = editor.LevelContainer.PeopleContainer;
                                    (item as Person).RandomizeAppearance();
                                    break;
                                default:
                                case LevelContainer.LayerType.misc:
                                    container = editor.LevelContainer.MiscContainer;
                                    break;
                            }

                            item.transform.parent = container.transform;
                            item.transform.localPosition = editor.MouseGridPosition;
                        }
                    }
                    else
                    {
                        SelectedItem = hoverItem;

                        if (!isInitialClick)
                            dragItem = true;
                    }
                }
                else
                {
                    if (SelectedItem != null)
                    {
                        selectedItem.transform.localPosition = editor.MouseGridPosition;
                        if (OnSelectionMoved != null)
                            OnSelectionMoved(selectedItem);
                    }
                    else
                    {
                        dragItem = false;
                    }
                }
            }

            isInitialClick = true;
        }

        if (InputUtility.RightMouse() || (InputUtility.LeftMouse() && editor.DeleteMode))
        {
            if (!editor.MouseInEditor())
            {
                if (hoverItem != null && !(hoverItem is Player))
                    DestroyItem(hoverItem);
                else
                    SelectedItem = null;
            }
        }

        if (!InputUtility.LeftMouse())
        {
            dragItem = false;
            isInitialClick = false;
        }
    }

    private void LevelOptionsControl()
    {
        if (!editor.MouseInEditor() && InputUtility.LeftMouse())
        {
            var delta = editor.MouseWorldPosition - editor.PreviousMouseWorldPosition;

            switch (leveltranslationMode)
            {
                default:
                case LevelTranslationMode.none:
                    break;
                //case LevelTranslationMode.background:
                //	LevelContainer.BackgroundsContainer.transform.position += delta;
                //	break;
                case LevelTranslationMode.phone:
                    editor.LevelContainer.Phone.transform.position += delta;
                    break;
            }
        }
    }

    private void CanvasControl()
    {
        if (editor.LevelContainer.InitializedCanvases)
            canvasEditor.Update();
    }

    public void Copy()
    {
        copiedItem = selectedItem;
    }
    public void Paste()
    {
        if (copiedItem)
        {
            var newItem = GameObject.Instantiate(copiedItem);
            newItem.transform.parent = copiedItem.transform.parent;

            Vector3 currentPosition = newItem.transform.position;
            RaycastHit[] hits;
            while (true)
            {
                var ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(currentPosition));
                hits = Physics.RaycastAll(ray, float.PositiveInfinity, Camera.main.cullingMask);

                bool found = false;
                foreach (var hit in hits)
                {
                    var item = hit.transform.GetComponentInParent<DWPObject>();
                    if (item != null && item.Info.Layer == newItem.Info.Layer)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    break;

                currentPosition += Vector3.right;
            }

            newItem.transform.position = currentPosition;
            SelectedItem = newItem;

            if (newItem.Info.Layer == LevelContainer.LayerType.people)
                UpdatePeoplePaths();
        }
    }

    private void DestroyItem(DWPObject item)
    {
        if (SelectedItem == item)
        {
            var previousSelectedItem = SelectedItem;
            SelectedItem = null;
            OnSelectionChanged(previousSelectedItem, null);
        }

        bool updatePeoplePaths = (item as NPC) != null;

        GameObject.Destroy(item.gameObject);

        if (updatePeoplePaths)
            UpdatePeoplePaths();
    }

    public void UpdatePeoplePaths()
    {
        peoplePaths.Clear();
        selectedPersonPath.Clear();

        var people = editor.LevelContainer.GetComponentsInChildren<NPC>();

        for (int i = 0; i < 2; i++)
        {
            foreach (var person in people)
            {
                if ((i == 0 && SelectedItem == person) || (i == 1 && SelectedItem != person))
                    continue;

                var path = (i == 0) ? peoplePaths : selectedPersonPath;

                if (person.enabled && person.Actions.Count > 0)
                {
                    Vector3 lastPosition = person.transform.position;

                    foreach (var action in person.Actions)
                    {
                        if (action.direction != ObjectAction.MovementDirection.None)
                        {
                            var nextPosition = lastPosition + action.DirectionVector * action.distance;

                            path.AddPoint(lastPosition);
                            path.AddPoint(nextPosition);

                            const float arrowDistanceAlongLine = 0.6f;
                            const float arrowSize = 0.1f;

                            Vector3 lineVec = (nextPosition - lastPosition).normalized;
                            Vector3 lineNormal = new Vector3(lineVec.y, lineVec.x, lineVec.z);
                            Vector3 arrowCenter = lastPosition + lineVec * arrowDistanceAlongLine * (nextPosition - lastPosition).magnitude;
                            Vector3 leftArrowPosition = arrowCenter - (lineVec + lineNormal) * arrowSize;
                            Vector3 rightArrowPosition = arrowCenter - (lineVec - lineNormal) * arrowSize;

                            path.AddPoint(arrowCenter);
                            path.AddPoint(leftArrowPosition);
                            path.AddPoint(arrowCenter);
                            path.AddPoint(rightArrowPosition);

                            lastPosition = nextPosition;
                        }
                    }
                }
            }
        }
    }

    public void OnPlayModeChanged(bool value)
    {
        if (!value)
            UpdatePeoplePaths();
    }

    public void OpenLevel(string path, LevelContainer levelContainer)
    {
        SelectedItem = null;

        /*#if UNITY_EDITOR

            if (path.Contains(".prefab")) 
            {
                Destroy(LevelContainer.gameObject);

                const string resources = "/Resources/";
                const string dotPrefab = ".prefab";
                string resourcePath = path.Substring(path.LastIndexOf(resources) + resources.Length);
                var prefab = Resources.Load<LevelContainer>(resourcePath.Substring(0, resourcePath.Length - dotPrefab.Length));

                LevelContainer = Instantiate(prefab);
            }
            else if (path.Contains(".txt"))
            {
                MobileIOUtility.OpenLevel(path, LevelContainer);
            }

        #else*/

        LevelIO.OpenLevel(path, levelContainer);

        //#endif

        EditingLayer = layerMode;

        UpdatePeoplePaths();

        if (OnOpenLevel != null)
            OnOpenLevel();
    }
}