using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelBlocks : EditorView
{
	[SerializeField]
	private Toggle seeOverToggle;

	[SerializeField]
	private GameObject selectedItemSection;

    [SerializeField]
    private Toggle defaultSeeOverToggle;

    [SerializeField]
    private Toggle displayInPlayModeToggle;

	private bool lockListeners = false;

	protected override void Start()
	{
		base.Start();

		selectedItemSection.SetActive(false);

		seeOverToggle.onValueChanged.AddListener(SeeOverToggle_OnValueChanged);

		editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;

        defaultSeeOverToggle.onValueChanged.AddListener(DefaultSeeOverToggle_OnValueChanged);
        displayInPlayModeToggle.onValueChanged.AddListener(DisplayInPlayModeToggle_OnValueChanged);
	}

    void OnDestroy()
	{
		seeOverToggle.onValueChanged.AddListener(SeeOverToggle_OnValueChanged);

		if (editor != null)
			editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;

        defaultSeeOverToggle.onValueChanged.RemoveListener(DefaultSeeOverToggle_OnValueChanged);
        displayInPlayModeToggle.onValueChanged.RemoveListener(DisplayInPlayModeToggle_OnValueChanged);
    }

	private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
	{
		if (lockListeners)
			return;

		var block = newItem as Block;

		selectedItemSection.SetActive(block != null);

		if (block != null)
		{
			lockListeners = true;
			seeOverToggle.isOn = block.CanSeeOver;
			lockListeners = false;
		}
	}

	private void DisplayBlocksToggle_OnValueChanged(bool value)
	{
		if (lockListeners)
			return;
	}

	private void SeeOverToggle_OnValueChanged(bool value)
	{
		if (lockListeners)
			return;

		var block = editor.LevelEditor.SelectedItem as Block;

		if (block != null)
			block.CanSeeOver = value;
	}

    private void DefaultSeeOverToggle_OnValueChanged(bool value)
    {
        editor.LevelEditor.DefaultSeeOverBlocks = value;
    }
    private void DisplayInPlayModeToggle_OnValueChanged(bool value)
    {
        GlobalData.displayBlocksInPlayMode = value;
    }
}