using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelPeopleActions : EditorView
{
    [SerializeField]
    private GameObject actionsSection;

    [SerializeField]
    private Button AddActionButton;

    [SerializeField]
    private Text durationText;

    private Dictionary<ObjectAction, PersonActionUI> actions = new Dictionary<ObjectAction, PersonActionUI>();

    private NPC Person
    {
        get
        {
            return editor.LevelEditor.SelectedItem as NPC;
        }
    }

    protected override void Start()
    {
        base.Start();

        AddActionButton.onClick.AddListener(AddActionButton_OnClick);

        editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;
        editor.LevelEditor.OnSelectionMoved += Editor_OnSelectionMoved;

        UpdateTotalDurationText();
    }

    void OnDestroy()
    {
        AddActionButton.onClick.RemoveListener(AddActionButton_OnClick);

        if (editor != null)
        {
            editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
            editor.LevelEditor.OnSelectionMoved -= Editor_OnSelectionMoved;
        }

        foreach (var action in actions.Values)
            Destroy(action);
    }

    private void OnPersonSelected()
    {
        var person = Person;

        foreach (var action in actions.Values)
            Destroy(action.gameObject);

        actions.Clear();

        foreach (var action in person.Actions)
            AddAction(action);
    }

    private ObjectAction AddAction(ObjectAction action = null)
    {
        var actionUI = Instantiate(Resources.Load<PersonActionUI>("Prefabs/EditorPrefabs/PersonActionUI"));

        actionUI.transform.parent = actionsSection.transform;

        if (action == null)
            action = new ObjectAction();

        actionUI.OnOrderArrowUpClicked += ActionUI_OnOrderArrowUpClicked;
        actionUI.OnOrderArrowDownClicked += ActionUI_OnOrderArrowDownClicked;

        actionUI.OnDeleteButtonClicked += ActionUI_OnDeleteButtonClicked;

        actionUI.OnPathChanged += ActionUI_OnPathChanged;

        actionUI.Action = action;

        actionUI.Direction = action.direction;

        actionUI.Expanded = action.expanded;

        actions.Add(action, actionUI);

        UpdatePath();

        return action;
    }

    private void RemoveAction(PersonActionUI actionUI)
    {
        actionUI.OnOrderArrowUpClicked -= ActionUI_OnOrderArrowUpClicked;
        actionUI.OnOrderArrowDownClicked -= ActionUI_OnOrderArrowDownClicked;

        actionUI.OnDeleteButtonClicked -= ActionUI_OnDeleteButtonClicked;

        actionUI.OnPathChanged -= ActionUI_OnPathChanged;

        actions.Remove(actionUI.Action);
        Person.Actions.Remove(actionUI.Action);

        Destroy(actionUI.gameObject);
    }

    private void AddActionButton_OnClick()
    {
        Person.Actions.Add(AddAction());
    }

    private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
    {
        var person = Person;

        if (person != null)
            OnPersonSelected();
    }
    private void Editor_OnSelectionMoved(DWPObject obj)
    {
        if (Person != null)
            UpdatePath();
    }

    private void ActionUI_OnOrderArrowUpClicked(PersonActionUI actionUI)
    {
        int index = Person.Actions.IndexOf(actionUI.Action);

        if (index > 0)
        {
            actionUI.transform.SetSiblingIndex(index - 1);
            actions[Person.Actions[index - 1]].transform.SetSiblingIndex(index);

            var temp = Person.Actions[index - 1];
            Person.Actions[index - 1] = Person.Actions[index];
            Person.Actions[index] = temp;
        }
    }
    private void ActionUI_OnOrderArrowDownClicked(PersonActionUI actionUI)
    {
        int index = Person.Actions.IndexOf(actionUI.Action);

        if (index < Person.Actions.Count - 1)
        {
            actionUI.transform.SetSiblingIndex(index + 1);
            actions[Person.Actions[index + 1]].transform.SetSiblingIndex(index);

            var temp = Person.Actions[index + 1];
            Person.Actions[index + 1] = Person.Actions[index];
            Person.Actions[index] = temp;
        }
    }

    private void ActionUI_OnDeleteButtonClicked(PersonActionUI actionUI)
    {
        RemoveAction(actionUI);
    }

    private void ActionUI_OnPathChanged()
    {
        UpdatePath();
    }

    private void UpdatePath()
    {
        editor.LevelEditor.UpdatePeoplePaths();
        UpdateTotalDurationText();
    }

    private void UpdateTotalDurationText()
    {
        float totalDuration = 0.0f;
        foreach (var action in Person.Actions)
        {
            float duration = 1.0f;

            if (action.wait)
                duration = action.waitTime;
            else
                duration = action.distance / (NPC.speed * action.speedMultiplier);
            
            totalDuration += duration;
        }

        durationText.text = "Duration: " + totalDuration + "s";
    }
}