using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelPeopleProperties : EditorView
{
    [SerializeField]
    private GameObject propertiesSection;

    [SerializeField]
    private Dropdown PropertyTypeDropdown;

    [SerializeField]
    private Button AddPropertyButton;

    private Dictionary<PersonProperty, PersonPropertyUI> properties = new Dictionary<PersonProperty, PersonPropertyUI>();

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

        PropertyTypeDropdown.onValueChanged.AddListener(PropertyTypeDropDown_OnValueChanged);
        AddPropertyButton.onClick.AddListener(AddPropertyButton_OnClick);

        editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;

        var options = new List<string>();
        foreach (PersonProperty.Type type in Enum.GetValues(typeof(PersonProperty.Type)))
            options.Add(type.ToString());
        PropertyTypeDropdown.AddOptions(options);
    }

    void OnDestroy()
    {
        PropertyTypeDropdown.onValueChanged.RemoveListener(PropertyTypeDropDown_OnValueChanged);
        AddPropertyButton.onClick.RemoveListener(AddPropertyButton_OnClick);

        if (editor != null)
        {
            editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
        }

        foreach (var property in properties.Values)
            Destroy(property);
    }

    private void OnPersonSelected()
    {
        var person = Person;

        foreach (var property in properties.Values)
            Destroy(property.gameObject);

        properties.Clear();

        foreach (var property in person.GetComponentsInChildren<PersonProperty>())
            AddProperty(property);
    }

    private PersonProperty AddProperty(PersonProperty property)
    {
        var propertyUI = Instantiate(Resources.Load<PersonPropertyUI>("Prefabs/EditorPrefabs/PersonPropertyUI"));

        propertyUI.transform.parent = propertiesSection.transform;

        propertyUI.OnOrderArrowUpClicked += PropertyUI_OnOrderArrowUpClicked;
        propertyUI.OnOrderArrowDownClicked += PropertyUI_OnOrderArrowDownClicked;

        propertyUI.OnDeleteButtonClicked += PropertyUI_OnDeleteButtonClicked;

        propertyUI.Property = property;

        propertyUI.Expanded = property.expanded;

        properties.Add(property, propertyUI);

        return property;
    }

    private void RemoveProperty(PersonPropertyUI propertyUI)
    {
        propertyUI.OnOrderArrowUpClicked -= PropertyUI_OnOrderArrowUpClicked;
        propertyUI.OnOrderArrowDownClicked -= PropertyUI_OnOrderArrowDownClicked;

        propertyUI.OnDeleteButtonClicked -= PropertyUI_OnDeleteButtonClicked;

        properties.Remove(propertyUI.Property);
        Destroy(propertyUI.Property.gameObject);

        Destroy(propertyUI.gameObject);
    }

    private void MoveProperty(PersonPropertyUI propertyUI, int direction)
    {
        int oldIndex = propertyUI.transform.GetSiblingIndex();
        int newIndex = oldIndex + direction;

        var properties = Person.Properties;
        if (newIndex >= 0 && newIndex < properties.Length)
        {
            var property = properties[oldIndex];
            var otherProperty = properties[newIndex];

            property.transform.SetSiblingIndex(newIndex);
            otherProperty.transform.SetSiblingIndex(oldIndex);

            var propertyUIs = propertyUI.transform.parent.GetComponentsInChildren<PersonPropertyUI>();
            var otherPropertyUI = propertyUIs[newIndex];

            propertyUI.transform.SetSiblingIndex(newIndex);
            otherPropertyUI.transform.SetSiblingIndex(oldIndex);
        }
    }

    private void RandomizeButton_OnClick()
    {
        foreach (var property in properties.Values)
            Destroy(property.gameObject);

        properties.Clear();
    }

    private void PropertyTypeDropDown_OnValueChanged(int index)
    {
    }
    private void AddPropertyButton_OnClick()
    {
        AddProperty(Person.AddProperty(PropertyTypeDropdown.options[PropertyTypeDropdown.value].text));
    }

    private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
    {
        var person = Person;

        if (person != null)
            OnPersonSelected();
    }

    private void PropertyUI_OnOrderArrowUpClicked(PersonPropertyUI propertyUI)
    {
        MoveProperty(propertyUI, -1);
    }
    private void PropertyUI_OnOrderArrowDownClicked(PersonPropertyUI propertyUI)
    {
        MoveProperty(propertyUI, 1);
    }

    private void PropertyUI_OnDeleteButtonClicked(PersonPropertyUI propertyUI)
    {
        RemoveProperty(propertyUI);
    }
}