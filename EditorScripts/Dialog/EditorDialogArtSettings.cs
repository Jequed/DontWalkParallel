using UnityEngine;
using UnityEngine.UI;

public class EditorDialogArtSettings : EditorDialog
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private SliderAndInputField jitterSpreadSlider;

    [SerializeField]
    private SliderAndInputField jitterSampleDistanceSlider;

    [SerializeField]
    private SliderAndInputField lineMinForceLengthSlider;

    [SerializeField]
    private SliderAndInputField lineMaxForceLengthSlider;

    [SerializeField]
    private SliderAndInputField lineForceSizeSlider;

    [SerializeField]
    private SliderAndInputField lineMaxSizeSlider;

    void Start()
    {
        button.onClick.AddListener(Button_OnClick);
        closeButton.onClick.AddListener(CloseButton_OnClick);

        jitterSpreadSlider.OnValueChanged += JitterSpreadSlider_OnValueChanged;
        jitterSampleDistanceSlider.OnValueChanged += JitterSampleDistanceSlider_OnValueChanged;

        lineMinForceLengthSlider.OnValueChanged += LineMinForceLengthSlider_OnValueChanged;
        lineMaxForceLengthSlider.OnValueChanged += LineMaxForceLengthSlider_OnValueChanged;
        lineForceSizeSlider.OnValueChanged += LineForceSizeSlider_OnValueChanged;
        lineMaxSizeSlider.OnValueChanged += LineMaxSizeSlider_OnValueChanged;


        jitterSpreadSlider.Value = editor.LevelEditor.CanvasEditor.MaxJitterSpread;
        jitterSampleDistanceSlider.Value = editor.LevelEditor.CanvasEditor.JitterSampleDistance;

        lineMinForceLengthSlider.Value = editor.LevelEditor.CanvasEditor.LineMinForceLength;
        lineMaxForceLengthSlider.Value = editor.LevelEditor.CanvasEditor.LineMaxForceLength;
        lineForceSizeSlider.Value = editor.LevelEditor.CanvasEditor.LineForceSize;
        lineMaxSizeSlider.Value = editor.LevelEditor.CanvasEditor.LineMaxSize;
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
        closeButton.onClick.RemoveListener(CloseButton_OnClick);

        jitterSpreadSlider.OnValueChanged -= JitterSpreadSlider_OnValueChanged;
        jitterSampleDistanceSlider.OnValueChanged -= JitterSampleDistanceSlider_OnValueChanged;

        lineMinForceLengthSlider.OnValueChanged -= LineMinForceLengthSlider_OnValueChanged;
        lineMaxForceLengthSlider.OnValueChanged -= LineMaxForceLengthSlider_OnValueChanged;
        lineForceSizeSlider.OnValueChanged -= LineForceSizeSlider_OnValueChanged;
        lineMaxSizeSlider.OnValueChanged -= LineMaxSizeSlider_OnValueChanged;
    }

    private void JitterSpreadSlider_OnValueChanged(float value)
    {
    }
    private void JitterSampleDistanceSlider_OnValueChanged(float value)
    {
    }

    private void LineMinForceLengthSlider_OnValueChanged(float value)
    {
    }
    private void LineMaxForceLengthSlider_OnValueChanged(float value)
    {
    }
    private void LineForceSizeSlider_OnValueChanged(float value)
    {
    }
    private void LineMaxSizeSlider_OnValueChanged(float value)
    {
    }

    private void Button_OnClick()
    {
        editor.LevelEditor.CanvasEditor.MaxJitterSpread = jitterSpreadSlider.Value;
        editor.LevelEditor.CanvasEditor.JitterSampleDistance = jitterSampleDistanceSlider.Value;

        editor.LevelEditor.CanvasEditor.LineMinForceLength = lineMinForceLengthSlider.Value;
        editor.LevelEditor.CanvasEditor.LineMaxForceLength = lineMaxForceLengthSlider.Value;
        editor.LevelEditor.CanvasEditor.LineForceSize = lineForceSizeSlider.Value;
        editor.LevelEditor.CanvasEditor.LineMaxSize = lineMaxSizeSlider.Value;

        editor.HideDialog();
    }
    private void CloseButton_OnClick()
    {
        editor.HideDialog();
    }
}