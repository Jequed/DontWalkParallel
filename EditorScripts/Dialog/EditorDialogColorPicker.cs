using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorDialogColorPicker : EditorDialog
{
    [SerializeField]
    private Image hueImage;

    [SerializeField]
    private Image valueSaturationImage;

    [SerializeField]
    private Image previewImage;

    [SerializeField]
    private Image alphaImage;

    [SerializeField]
    private Image hueSelectorImage;

    [SerializeField]
    private Image valueSaturationSelectorImage;

    [SerializeField]
    private Image alphaSelectorImage;

    [SerializeField]
    private InputField redInputField;

    [SerializeField]
    private InputField greenInputField;

    [SerializeField]
    private InputField blueInputField;

    [SerializeField]
    private InputField alphaInputField;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Button closeButton;

    private Canvas canvas;

    private float hue = 0.0f;
    private float value = 0.0f;
    private float saturation = 0.0f;
    private float alpha = 1.0f;

    private bool draggingHue = false;
    private bool draggingValueSaturation = false;
    private bool draggingAlpha = false;

    private bool isPrimary;

    public bool IsPrimary
    {
        get
        {
            return isPrimary;
        }
        set
        {
            isPrimary = value;
        }
    }

    private void OnEnable()
    {
        var color = new HSBColor(isPrimary ? editor.LevelEditor.CanvasEditor.PrimaryColor : editor.LevelEditor.CanvasEditor.SecondaryColor);
        hue = color.h;
        value = color.b;
        saturation = color.s;
        alpha = color.a;

        UpdateColors();
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        redInputField.onEndEdit.AddListener(RedInputField_OnEndEdit);
        greenInputField.onEndEdit.AddListener(GreenInputField_OnEndEdit);
        blueInputField.onEndEdit.AddListener(BlueInputField_OnEndEdit);
        alphaInputField.onEndEdit.AddListener(AlphaInputField_OnEndEdit);

        button.onClick.AddListener(Button_OnClick);
        closeButton.onClick.AddListener(CloseButton_OnClick);

        UpdateColors();
    }

    private void OnDestroy()
    {
        redInputField.onEndEdit.RemoveListener(RedInputField_OnEndEdit);
        greenInputField.onEndEdit.RemoveListener(GreenInputField_OnEndEdit);
        blueInputField.onEndEdit.RemoveListener(BlueInputField_OnEndEdit);
        alphaInputField.onEndEdit.RemoveListener(AlphaInputField_OnEndEdit);

        button.onClick.RemoveListener(Button_OnClick);
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
    }

    void Update()
    {
        UpdateHueImage();
        UpdateValueSaturationImage();
        UpdateAlphaImage();
    }

    private void UpdateHueImage()
    {
        Vector3 bottomLeft, topRight;
        bool inImage = UIUtility.InImage(InputUtility.MousePosition, hueImage, out bottomLeft, out topRight);

        if (InputUtility.LeftMouseDown() && inImage)
            draggingHue = true;

        if (InputUtility.LeftMouseUp())
            draggingHue = false;


        var position = hueSelectorImage.transform.position;
        var sub = topRight.x - bottomLeft.x;

        if (draggingHue)
        {
            hue = Mathf.Clamp01((InputUtility.MousePosition.x - bottomLeft.x) / sub);

            UpdateColors();
        }

        position.x = bottomLeft.x + sub * hue;
        position.x = canvas.worldCamera.ScreenToWorldPoint(position).x;

        hueSelectorImage.transform.position = position;
    }
    private void UpdateValueSaturationImage()
    {
        Vector3 bottomLeft, topRight;
        bool inImage = UIUtility.InImage(InputUtility.MousePosition, valueSaturationImage, out bottomLeft, out topRight);

        if (InputUtility.LeftMouseDown() && inImage)
            draggingValueSaturation = true;

        if (InputUtility.LeftMouseUp())
            draggingValueSaturation = false;


        var position = valueSaturationSelectorImage.transform.position;
        var sub = topRight - bottomLeft;

        if (draggingValueSaturation)
        {
            saturation = Mathf.Clamp01((InputUtility.MousePosition.x - bottomLeft.x) / sub.x);
            value = Mathf.Clamp01((InputUtility.MousePosition.y - bottomLeft.y) / sub.y);

            UpdateColors();
        }

        position = canvas.worldCamera.ScreenToWorldPoint(bottomLeft + new Vector3(saturation * sub.x, value * sub.y));

        valueSaturationSelectorImage.transform.position = new Vector3(position.x, position.y, valueSaturationSelectorImage.transform.position.z);
    }
    private void UpdateAlphaImage()
    {
        Vector3 bottomLeft, topRight;
        bool inImage = UIUtility.InImage(InputUtility.MousePosition, alphaImage, out bottomLeft, out topRight);

        if (InputUtility.LeftMouseDown() && inImage)
            draggingAlpha = true;

        if (InputUtility.LeftMouseUp())
            draggingAlpha = false;


        var position = alphaSelectorImage.transform.position;
        var sub = topRight.y - bottomLeft.y;

        if (draggingAlpha)
        {
            alpha = Mathf.Clamp01((InputUtility.MousePosition.y - bottomLeft.y) / sub);

            UpdateColors();
        }

        position.y = bottomLeft.y + sub * alpha;
        position.y = canvas.worldCamera.ScreenToWorldPoint(position).y;

        alphaSelectorImage.transform.position = position;
    }

    private void UpdateColors()
    {
        var finalColor = new HSBColor(hue, saturation, value, alpha).ToColor();

        valueSaturationImage.material.SetColor("_Color", new HSBColor(hue, 1.0f, 1.0f, 1.0f).ToColor());
        alphaImage.color = new HSBColor(hue, saturation, value, 1.0f).ToColor();
        previewImage.color = finalColor;

        redInputField.text = FloatToInt(finalColor.r).ToString();
        greenInputField.text = FloatToInt(finalColor.g).ToString();
        blueInputField.text = FloatToInt(finalColor.b).ToString();
        alphaInputField.text = FloatToInt(finalColor.a).ToString();
    }

    private void UpdateColorFromInputFields()
    {
        try
        {
            if (redInputField.text == "")
                redInputField.text = "0";
            if (greenInputField.text == "")
                greenInputField.text = "0";
            if (blueInputField.text == "")
                blueInputField.text = "0";
            if (alphaInputField.text == "")
                alphaInputField.text = "0";

            float r = IntToFloat(Mathf.Clamp(int.Parse(redInputField.text), 0, 255));
            float g = IntToFloat(Mathf.Clamp(int.Parse(greenInputField.text), 0, 255));
            float b = IntToFloat(Mathf.Clamp(int.Parse(blueInputField.text), 0, 255));
            float a = IntToFloat(Mathf.Clamp(int.Parse(alphaInputField.text), 0, 255));

            var color = new HSBColor(new Color(r, g, b, a));

            hue = color.h;
            value = color.b;
            saturation = color.s;
            alpha = color.a;

            UpdateColors();
        }
        catch (FormatException)
        {
        }
    }

    private void RedInputField_OnEndEdit(string value)
    {
        UpdateColorFromInputFields();
    }
    private void GreenInputField_OnEndEdit(string value)
    {
        UpdateColorFromInputFields();
    }
    private void BlueInputField_OnEndEdit(string value)
    {
        UpdateColorFromInputFields();
    }
    private void AlphaInputField_OnEndEdit(string value)
    {
        UpdateColorFromInputFields();
    }

    private void Button_OnClick()
    {
        if (isPrimary)
            editor.LevelEditor.CanvasEditor.PrimaryColor = previewImage.color;
        else
            editor.LevelEditor.CanvasEditor.SecondaryColor = previewImage.color;

        editor.HideDialog();
    }
    private void CloseButton_OnClick()
    {
        editor.HideDialog();
    }

    private static int FloatToInt(float f)
    {
        return (int)(f * 255.0f);
    }
    private static float IntToFloat(int i)
    {
        return (float)i / 255.0f;
    }
}