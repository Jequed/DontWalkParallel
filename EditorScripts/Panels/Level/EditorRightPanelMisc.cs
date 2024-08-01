using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelMisc : EditorView
{
	[SerializeField]
	private Dropdown itemTypeDropdown;

	[SerializeField]
	private GameObject itemOptions;

	private GameObject workspace = null;

	private List<ItemInfo.Type> miscTypes = new List<ItemInfo.Type>();

	protected override void Start()
	{
		base.Start();

		foreach (ItemInfo.Type type in Enum.GetValues(typeof(ItemInfo.Type)))
		{
			var info = GlobalData.GetInfo(type);
			if (info != null && type != ItemInfo.Type.None && info.Layer == LevelContainer.LayerType.misc)
				miscTypes.Add(type);
		}

		var options = new List<string>();
		foreach (var type in miscTypes)
			options.Add(type.ToString());
		itemTypeDropdown.AddOptions(options);

		itemTypeDropdown.onValueChanged.AddListener(ItemTypeDropdown_OnValueChanged);

		editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;
	}

	void OnDestroy()
	{
		itemTypeDropdown.onValueChanged.RemoveListener(ItemTypeDropdown_OnValueChanged);

		if (editor != null)
			editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
	}

	private void OnItemSelected()
	{
		if (workspace != null)
			Destroy(workspace);

		var resource = Resources.Load<GameObject>(editor.LevelEditor.SelectedItem.Info.WorkspaceName);
		if (resource != null)
		{
			workspace = Instantiate(resource);
			workspace.transform.parent = itemOptions.transform;
		}
	}

	private void ItemTypeDropdown_OnValueChanged(int value)
	{
		editor.LevelEditor.CurrentItemType = miscTypes[value];
	}

	private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
	{
		if (newItem != null)
		{
			itemOptions.SetActive(newItem.Layer == LevelContainer.LayerType.misc);

			if (newItem.Layer == LevelContainer.LayerType.misc)
				OnItemSelected();
		}
		else
		{
			itemOptions.SetActive(false);
		}
	}
}