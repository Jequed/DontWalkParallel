using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelPeopleAccessories : EditorView
{
    [SerializeField]
    private GameObject accessoriesSection;

    [SerializeField]
    private Button RandomizeButton;

    [SerializeField]
    private Dropdown AccessoryTypeDropdown;

    [SerializeField]
    private Button AddAccessoryButton;

    private Dictionary<PersonAccessoryInstance, PersonAccessoryUI> accessories = new Dictionary<PersonAccessoryInstance, PersonAccessoryUI>();

    private Person Person
    {
        get
        {
            return editor.LevelEditor.SelectedItem as Person;
        }
    }

    protected override void Start()
    {
        base.Start();

        RandomizeButton.onClick.AddListener(RandomizeButton_OnClick);

        AccessoryTypeDropdown.onValueChanged.AddListener(AccessoryTypeDropDown_OnValueChanged);
        AddAccessoryButton.onClick.AddListener(AddAccessoryButton_OnClick);

        editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;

        var options = new List<string>();
        foreach (var accessory in PersonData.accessories)
            options.Add(accessory.name);
        AccessoryTypeDropdown.AddOptions(options);
    }

    void OnDestroy()
    {
        RandomizeButton.onClick.RemoveListener(RandomizeButton_OnClick);

        AccessoryTypeDropdown.onValueChanged.RemoveListener(AccessoryTypeDropDown_OnValueChanged);
        AddAccessoryButton.onClick.RemoveListener(AddAccessoryButton_OnClick);

        if (editor != null)
        {
            editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
        }

        foreach (var accessory in accessories.Values)
            Destroy(accessory);
    }

    private void OnPersonSelected()
    {
        var person = Person;

        foreach (var accessory in accessories.Values)
            Destroy(accessory.gameObject);

        accessories.Clear();

        foreach (var accessory in person.AppearanceManager.Accessories)
            AddAcccesory(accessory);
    }

    private PersonAccessoryInstance AddAcccesory(PersonAccessoryInstance accessoryInstance = null)
    {
        var accessoryUI = Instantiate(Resources.Load<PersonAccessoryUI>("Prefabs/EditorPrefabs/PersonAccessoryUI"));

        accessoryUI.transform.parent = accessoriesSection.transform;

        if (accessoryInstance == null)
            accessoryInstance = new PersonAccessoryInstance();

        accessoryUI.OnOrderArrowUpClicked += AccessoryUI_OnOrderArrowUpClicked;
        accessoryUI.OnOrderArrowDownClicked += AccessoryUI_OnOrderArrowDownClicked;

        accessoryUI.OnDeleteButtonClicked += AccessoryUI_OnDeleteButtonClicked;

        accessoryUI.AccessoryInstance = accessoryInstance;

        accessoryUI.Expanded = accessoryInstance.expanded;

        accessories.Add(accessoryInstance, accessoryUI);

        return accessoryInstance;
    }

    private void RemoveAccessory(PersonAccessoryUI accessoryUI)
    {
        accessoryUI.OnOrderArrowUpClicked -= AccessoryUI_OnOrderArrowUpClicked;
        accessoryUI.OnOrderArrowDownClicked -= AccessoryUI_OnOrderArrowDownClicked;

        accessoryUI.OnDeleteButtonClicked -= AccessoryUI_OnDeleteButtonClicked;

        accessories.Remove(accessoryUI.AccessoryInstance);
        Person.AppearanceManager.RemoveAccessory(accessoryUI.AccessoryInstance);

        Destroy(accessoryUI.gameObject);
    }

    private void RandomizeButton_OnClick()
    {
        foreach (var accessory in accessories.Values)
            Destroy(accessory.gameObject);

        accessories.Clear();

        Person.RandomizeAppearance();

        foreach (var accessory in Person.AppearanceManager.Accessories)
            AddAcccesory(accessory);
    }

    private void AccessoryTypeDropDown_OnValueChanged(int index)
    {
    }
    private void AddAccessoryButton_OnClick()
    {
        AddAcccesory(Person.AppearanceManager.AddAccessory(PersonData.accessories[AccessoryTypeDropdown.value]));
    }

    private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
    {
        var person = Person;

        if (person != null)
            OnPersonSelected();
    }

    private void AccessoryUI_OnOrderArrowUpClicked(PersonAccessoryUI accessoryUI)
    {
        int index = Person.AppearanceManager.Accessories.IndexOf(accessoryUI.AccessoryInstance);

        if (index > 0)
        {
            accessoryUI.transform.SetSiblingIndex(index - 1);
            accessoryUI.AccessoryInstance.transform.SetSiblingIndex(index - 1);
            accessories[Person.AppearanceManager.Accessories[index - 1]].transform.SetSiblingIndex(index);

            var temp = Person.AppearanceManager.Accessories[index - 1];
            Person.AppearanceManager.Accessories[index - 1] = Person.AppearanceManager.Accessories[index];
            Person.AppearanceManager.Accessories[index] = temp;
        }

        Person.AppearanceManager.RefreshZOrders();
    }
    private void AccessoryUI_OnOrderArrowDownClicked(PersonAccessoryUI accessoryUI)
    {
        int index = Person.AppearanceManager.Accessories.IndexOf(accessoryUI.AccessoryInstance);

        if (index < Person.AppearanceManager.Accessories.Count - 1)
        {
            accessoryUI.transform.SetSiblingIndex(index + 1);
            accessoryUI.AccessoryInstance.transform.SetSiblingIndex(index + 1);
            accessories[Person.AppearanceManager.Accessories[index + 1]].transform.SetSiblingIndex(index);

            var temp = Person.AppearanceManager.Accessories[index + 1];
            Person.AppearanceManager.Accessories[index + 1] = Person.AppearanceManager.Accessories[index];
            Person.AppearanceManager.Accessories[index] = temp;
        }

        Person.AppearanceManager.RefreshZOrders();
    }

    private void AccessoryUI_OnDeleteButtonClicked(PersonAccessoryUI accessoryUI)
    {
        RemoveAccessory(accessoryUI);
    }
}