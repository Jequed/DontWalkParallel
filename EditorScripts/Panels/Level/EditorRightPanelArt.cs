using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelArt : EditorView
{
    [SerializeField]
    private GameObject uninitializedSection;

    [SerializeField]
    private GameObject initializedSection;

    [SerializeField]
    private InputField widthInputField;

    [SerializeField]
    private InputField heightInputField;

    [SerializeField]
    private Button offsetArrowLeft;

    [SerializeField]
    private Button offsetArrowRight;

    [SerializeField]
    private Button offsetArrowUp;

    [SerializeField]
    private Button offsetArrowDown;

    [SerializeField]
    private Button createButton;

    [SerializeField]
    private GameObject selection;

    [SerializeField]
    private Button primaryColorButton;

    [SerializeField]
    private Button secondaryColorButton;

    [SerializeField]
    private SliderAndInputField brushSizeSlider;

    [SerializeField]
    private Button undoButton;

    [SerializeField]
    private Button redoButton;

    [SerializeField]
    private Button paintBrushButton;

    [SerializeField]
    private Button paintBucketButton;

    [SerializeField]
    private Button inkDropperButton;

    [SerializeField]
    private Button lineToolButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private Toggle foregroundToggle;

    [SerializeField]
    private Toggle shadowToggle;

    [SerializeField]
    private Toggle backgroundToggle;

    [SerializeField]
    private Toggle foregroundVisibilityToggle;

    [SerializeField]
    private Toggle shadowVisibilityToggle;

    [SerializeField]
    private Toggle backgroundVisibilityToggle;

    [SerializeField]
    private Toggle displayOverLevelToggle;


    private int width = 10;
    private int height = 10;
    private int xOffset = 0;
    private int yOffset = 0;

    private Vector3 previewBottomLeft;
    private Vector3 previewTopRight;

    private LineDrawer.LineCollection sizePreviewLines;

    private bool displayOverLevel = false;

    private const int maxWidth = 50;
    private const int maxHeight = 50;

    protected override void Start()
    {
        base.Start();

        sizePreviewLines = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(Color.blue));

        BrushSizeSlider_OnValueChanged(editor.LevelEditor.CanvasEditor.BrushSize);

        widthInputField.onEndEdit.AddListener(WidthInputField_OnValueChanged);
        heightInputField.onEndEdit.AddListener(HeightInputField_OnValueChanged);
        offsetArrowLeft.onClick.AddListener(OffsetArrowLeft_OnClick);
        offsetArrowRight.onClick.AddListener(OffsetArrowRight_OnClick);
        offsetArrowUp.onClick.AddListener(OffsetArrowUp_OnClick);
        offsetArrowDown.onClick.AddListener(OffsetArrowDown_OnClick);
        createButton.onClick.AddListener(CreateButton_OnClick);

        primaryColorButton.onClick.AddListener(PrimaryColorButton_OnClick);
        secondaryColorButton.onClick.AddListener(SecondaryColorButton_OnClick);
		brushSizeSlider.OnValueChanged += BrushSizeSlider_OnValueChanged;
        undoButton.onClick.AddListener(UndoButton_OnClick);
        redoButton.onClick.AddListener(RedoButton_OnClick);
        paintBrushButton.onClick.AddListener(PaintBrushButton_OnClick);
        paintBucketButton.onClick.AddListener(PaintBucketButton_OnClick);
        inkDropperButton.onClick.AddListener(InkDropperButton_OnClick);
        lineToolButton.onClick.AddListener(LineToolButton_OnClick);
        settingsButton.onClick.AddListener(SettingsButton_OnClick);

        foregroundToggle.onValueChanged.AddListener(ForegroundToggle_OnValueChanged);
        shadowToggle.onValueChanged.AddListener(ShadowToggle_OnValueChanged);
        backgroundToggle.onValueChanged.AddListener(BackgroundToggle_OnValueChanged);
        foregroundVisibilityToggle.onValueChanged.AddListener(ForegroundVisibilityToggle_OnValueChanged);
        shadowVisibilityToggle.onValueChanged.AddListener(ShadowVisibilityToggle_OnValueChanged);
        backgroundVisibilityToggle.onValueChanged.AddListener(BackgroundVisibilityToggle_OnValueChanged);

        displayOverLevelToggle.onValueChanged.AddListener(DisplayOverLevelToggle_OnValueChanged);

        editor.LevelEditor.CanvasEditor.OnColorChanged += CanvasEditor_OnColorChanged;

        editor.LevelEditor.OnOpenLevel += UpdatePanelVisibility;

        if (!editor.LevelContainer.InitializedCanvases)
        {
            widthInputField.text = "10";
            heightInputField.text = "10";
        }

        UpdatePanelVisibility();
    }

    void OnEnable()
    {
        xOffset = 0;
        yOffset = 0;

        UpdatePanelVisibility();
    }

    void OnDestroy()
    {
        widthInputField.onEndEdit.RemoveListener(WidthInputField_OnValueChanged);
        heightInputField.onEndEdit.RemoveListener(HeightInputField_OnValueChanged);
        createButton.onClick.RemoveListener(CreateButton_OnClick);

        primaryColorButton.onClick.RemoveListener(PrimaryColorButton_OnClick);
        secondaryColorButton.onClick.RemoveListener(SecondaryColorButton_OnClick);
        brushSizeSlider.OnValueChanged -= BrushSizeSlider_OnValueChanged;
        undoButton.onClick.RemoveListener(UndoButton_OnClick);
        redoButton.onClick.RemoveListener(RedoButton_OnClick);
        paintBrushButton.onClick.RemoveListener(PaintBrushButton_OnClick);
        paintBucketButton.onClick.RemoveListener(PaintBucketButton_OnClick);
        inkDropperButton.onClick.RemoveListener(InkDropperButton_OnClick);
        lineToolButton.onClick.RemoveListener(LineToolButton_OnClick);
        settingsButton.onClick.RemoveListener(SettingsButton_OnClick);

        foregroundToggle.onValueChanged.RemoveListener(ForegroundToggle_OnValueChanged);
        shadowToggle.onValueChanged.RemoveListener(ShadowToggle_OnValueChanged);
        backgroundToggle.onValueChanged.RemoveListener(BackgroundToggle_OnValueChanged);
        foregroundVisibilityToggle.onValueChanged.RemoveListener(ForegroundVisibilityToggle_OnValueChanged);
        shadowVisibilityToggle.onValueChanged.RemoveListener(ShadowVisibilityToggle_OnValueChanged);
        backgroundVisibilityToggle.onValueChanged.RemoveListener(BackgroundVisibilityToggle_OnValueChanged);

        displayOverLevelToggle.onValueChanged.RemoveListener(DisplayOverLevelToggle_OnValueChanged);

        editor.LevelEditor.CanvasEditor.OnColorChanged -= CanvasEditor_OnColorChanged;

        editor.LevelEditor.OnOpenLevel -= UpdatePanelVisibility;
    }

    void Update()
    {
        sizePreviewLines.Clear();

        if (!editor.LevelContainer.InitializedCanvases)
        {
            Vector3 size = new Vector3((float)width, (float)height);
            Vector3 offset = new Vector3((float)xOffset, (float)yOffset);
            previewBottomLeft = new Vector3(-size.x * 0.5f, -size.y * 0.5f) + offset;
            previewTopRight = new Vector3(size.x * 0.5f, size.y * 0.5f) + offset;

            if (width % 2 == 0)
            {
                previewBottomLeft.x -= 0.5f;
                previewTopRight.x -= 0.5f;
            }
            if (height % 2 == 0)
            {
                previewBottomLeft.y -= 0.5f;
                previewTopRight.y -= 0.5f;
            }

            sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x, previewBottomLeft.y)); sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x, previewTopRight.y));
            sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x, previewTopRight.y)); sizePreviewLines.AddPoint(new Vector3(previewTopRight.x, previewTopRight.y));
            sizePreviewLines.AddPoint(new Vector3(previewTopRight.x, previewTopRight.y)); sizePreviewLines.AddPoint(new Vector3(previewTopRight.x, previewBottomLeft.y));
            sizePreviewLines.AddPoint(new Vector3(previewTopRight.x, previewBottomLeft.y)); sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x, previewBottomLeft.y));

            var increment = Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(previewBottomLeft) + Vector3.right).x;

            sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x - increment, previewBottomLeft.y - increment)); sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x - increment, previewTopRight.y + increment));
            sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x - increment, previewTopRight.y)); sizePreviewLines.AddPoint(new Vector3(previewTopRight.x + increment, previewTopRight.y + increment));
            sizePreviewLines.AddPoint(new Vector3(previewTopRight.x + increment, previewTopRight.y)); sizePreviewLines.AddPoint(new Vector3(previewTopRight.x + increment, previewBottomLeft.y - increment));
            sizePreviewLines.AddPoint(new Vector3(previewTopRight.x + increment, previewBottomLeft.y - increment)); sizePreviewLines.AddPoint(new Vector3(previewBottomLeft.x - increment, previewBottomLeft.y - increment));
        }
    }

    private void UpdatePanelVisibility()
    {
        if (editor != null)
        {
            uninitializedSection.gameObject.SetActive(!editor.LevelContainer.InitializedCanvases);
            initializedSection.gameObject.SetActive(editor.LevelContainer.InitializedCanvases);

            UpdateIcons();
        }
    }
    private void UpdateIcons()
    {
        primaryColorButton.image.color = editor.LevelEditor.CanvasEditor.PrimaryColor;
        secondaryColorButton.image.color = editor.LevelEditor.CanvasEditor.SecondaryColor;

        switch (editor.LevelEditor.CanvasEditor.Tool)
        {
            case LevelCanvasEditor.ToolType.paintBrush:
                selection.transform.position = paintBrushButton.transform.position;
                break;
            case LevelCanvasEditor.ToolType.paintBucket:
                selection.transform.position = paintBucketButton.transform.position;
                break;
            case LevelCanvasEditor.ToolType.inkDropper:
                selection.transform.position = inkDropperButton.transform.position;
                break;
            case LevelCanvasEditor.ToolType.line:
                selection.transform.position = lineToolButton.transform.position;
                break;
        }

        foregroundToggle.isOn = editor.LevelEditor.CanvasEditor.CurrentCanvas == editor.LevelContainer.Canvas_Foreground;
        shadowToggle.isOn = editor.LevelEditor.CanvasEditor.CurrentCanvas == editor.LevelContainer.Canvas_Shadow;
        backgroundToggle.isOn = editor.LevelEditor.CanvasEditor.CurrentCanvas == editor.LevelContainer.Canvas_Background;

        foregroundVisibilityToggle.isOn = editor.LevelContainer.Canvas_Foreground.gameObject.activeSelf;
        shadowVisibilityToggle.isOn = editor.LevelContainer.Canvas_Shadow.gameObject.activeSelf;
        backgroundVisibilityToggle.isOn = editor.LevelContainer.Canvas_Background.gameObject.activeSelf;

        displayOverLevelToggle.isOn = displayOverLevel;
    }

    private void InitializeCanvas(DWPCanvas canvas, Color clearColor)
    {
        canvas.Initialize((float)width, (float)height, width * GlobalData.spritePixelSize, height * GlobalData.spritePixelSize);
        canvas.Clear(clearColor);

        var center = (previewBottomLeft + previewTopRight) * 0.5f;
        canvas.transform.position = new Vector3(center.x, center.y, canvas.transform.position.z);
    }

    private void WidthInputField_OnValueChanged(string value)
    {
        try
        {
            width = int.Parse(value);

            if (width < 1)
                width = 1;
            else if (width > maxWidth)
                width = maxWidth;

            widthInputField.text = width.ToString();
        }
        catch (FormatException)
        {
        }
    }
    private void HeightInputField_OnValueChanged(string value)
    {
        try
        {
            height = int.Parse(value);

            if (height < 1)
                height = 1;
            else if (height > maxHeight)
                height = maxHeight;

            heightInputField.text = height.ToString();
        }
        catch (FormatException)
        {
        }
    }
    private void OffsetArrowLeft_OnClick()
    {
        xOffset--;
    }
    private void OffsetArrowRight_OnClick()
    {
        xOffset++;
    }
    private void OffsetArrowUp_OnClick()
    {
        yOffset++;
    }
    private void OffsetArrowDown_OnClick()
    {
        yOffset--;
    }
    private void CreateButton_OnClick()
    {
        InitializeCanvas(editor.LevelContainer.Canvas_Foreground, Color.clear);
        InitializeCanvas(editor.LevelContainer.Canvas_Shadow, Color.clear);
        InitializeCanvas(editor.LevelContainer.Canvas_Background, Color.white);

        editor.LevelEditor.CanvasEditor.ResetPreviousColors();

        UpdatePanelVisibility();
    }

    private void PrimaryColorButton_OnClick()
    {
        editor.ShowDialogColorPicker(true);
    }
    private void SecondaryColorButton_OnClick()
    {
        editor.ShowDialogColorPicker(false);
    }
    private void BrushSizeSlider_OnValueChanged(float value)
    {
        editor.LevelEditor.CanvasEditor.BrushSize = (int)value;
    }
    private void UndoButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Undo();
    }
    private void RedoButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Redo();
    }
    private void PaintBrushButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Tool = LevelCanvasEditor.ToolType.paintBrush;
        UpdateIcons();
    }
    private void PaintBucketButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Tool = LevelCanvasEditor.ToolType.paintBucket;
        UpdateIcons();
    }
    private void InkDropperButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Tool = LevelCanvasEditor.ToolType.inkDropper;
        UpdateIcons();
    }
    private void LineToolButton_OnClick()
    {
        editor.LevelEditor.CanvasEditor.Tool = LevelCanvasEditor.ToolType.line;
        UpdateIcons();
    }
    private void SettingsButton_OnClick()
    {
        editor.ShowDialogArtSettings();
        UpdateIcons();
    }

    private void ForegroundToggle_OnValueChanged(bool value)
    {
        if (value)
            editor.LevelEditor.CanvasEditor.CurrentCanvas = editor.LevelContainer.Canvas_Foreground;
    }
    private void ShadowToggle_OnValueChanged(bool value)
    {
        if (value)
            editor.LevelEditor.CanvasEditor.CurrentCanvas = editor.LevelContainer.Canvas_Shadow;
    }
    private void BackgroundToggle_OnValueChanged(bool value)
    {
        if (value)
            editor.LevelEditor.CanvasEditor.CurrentCanvas = editor.LevelContainer.Canvas_Background;
    }
    private void ForegroundVisibilityToggle_OnValueChanged(bool value)
    {
        editor.LevelContainer.Canvas_Foreground.gameObject.SetActive(value);
    }
    private void ShadowVisibilityToggle_OnValueChanged(bool value)
    {
        editor.LevelContainer.Canvas_Shadow.gameObject.SetActive(value);
    }
    private void BackgroundVisibilityToggle_OnValueChanged(bool value)
    {
        editor.LevelContainer.Canvas_Background.gameObject.SetActive(value);
    }

    private void DisplayOverLevelToggle_OnValueChanged(bool value)
    {
        displayOverLevel = value;
        var position = editor.LevelContainer.BackgroundsContainer.transform.localPosition;
        position.z = value ? -5.0f : 0.0f;
        editor.LevelContainer.BackgroundsContainer.transform.localPosition = position;
    }

    private void CanvasEditor_OnColorChanged()
    {
        UpdateIcons();
    }
}