using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelLevel : EditorView
{
	[SerializeField]
	private SliderAndInputField screenSize;

	[SerializeField]
	private ToggleGroup levelTranslationType;

    [SerializeField]
    private InputField thoughtBubbleTextInputField;

	private List<Toggle> toggles;

    const float screenHeight = 7.18f; //718 pixels high

    protected override void Start()
	{
		base.Start();

        ScreenSize_OnValueChanged(screenSize.Value);

        UpdateParamters();

        editor.LevelEditor.OnOpenLevel += LevelEditor_OnOpenLevel;

		screenSize.OnValueChanged += ScreenSize_OnValueChanged;

        thoughtBubbleTextInputField.onValueChanged.AddListener(thoughtBubbleTextInputField_OnValueChanged);

        toggles = new List<Toggle>(levelTranslationType.GetComponentsInChildren<Toggle>());
		foreach (var toggle in toggles)
			toggle.onValueChanged.AddListener(LevelTranslationType_OnValueChanged);
	}

    void OnDestroy()
	{
        editor.LevelEditor.OnOpenLevel -= LevelEditor_OnOpenLevel;

        screenSize.OnValueChanged -= ScreenSize_OnValueChanged;

        thoughtBubbleTextInputField.onValueChanged.RemoveListener(thoughtBubbleTextInputField_OnValueChanged);

        foreach (var toggle in toggles)
			toggle.onValueChanged.RemoveListener(LevelTranslationType_OnValueChanged);
	}

    private void UpdateParamters()
    {
        thoughtBubbleTextInputField.text = editor.LevelContainer.ThoughtBubbleText;

        screenSize.Value = editor.LevelContainer.Phone.transform.localScale.x * screenHeight;
    }

    private void LevelEditor_OnOpenLevel()
    {
        UpdateParamters();
    }

    private void ScreenSize_OnValueChanged(float value)
	{
		editor.LevelContainer.Phone.transform.localScale = new Vector3(value / screenHeight, value / screenHeight, 1.0f);
	}

	private void LevelTranslationType_OnValueChanged(bool value)
	{
		for (int i = 0; i < toggles.Count; i++)
		{
			if (toggles[i].isOn)
			{
				editor.LevelEditor.LevelTranslateMode = (LevelEditor.LevelTranslationMode)i;
				break;
			}
		}
	}

    private void thoughtBubbleTextInputField_OnValueChanged(string value)
    {
        editor.LevelContainer.ThoughtBubbleText = value;
    }

    private Sprite LoadSprite(string path)
	{
		Sprite sprite = Resources.Load<Sprite>("Images/Backgrounds/" + path);

		return sprite;
	}
}