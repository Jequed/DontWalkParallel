using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelPeople : EditorView
{
	[SerializeField]
	private Button leftArrow;

	[SerializeField]
	private Button rightArrow;

	[SerializeField]
	private Button downArrow;

	[SerializeField]
	private Button upArrow;

	[SerializeField]
	private GameObject selectedItemSection;

    [SerializeField]
    private Button actionTabButton;

    [SerializeField]
    private Button accessoryTabButton;

    [SerializeField]
    private Button propertyTabButton;

    [SerializeField]
    private GameObject actionSection;

    [SerializeField]
    private GameObject accessorySection;

    [SerializeField]
    private GameObject propertySection;

    private ObjectAction.MovementDirection direction;

	private bool lockListeners = false;

	private readonly Color highlightedColor = Color.blue;
	private readonly Color nonHightlightedColor = Color.white;

	public ObjectAction.MovementDirection Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;

			Person.InitialDirection = value;

			leftArrow.targetGraphic.color = (direction == ObjectAction.MovementDirection.Left) ? highlightedColor : nonHightlightedColor;
			rightArrow.targetGraphic.color = (direction == ObjectAction.MovementDirection.Right) ? highlightedColor : nonHightlightedColor;
			downArrow.targetGraphic.color = (direction == ObjectAction.MovementDirection.Down) ? highlightedColor : nonHightlightedColor;
			upArrow.targetGraphic.color = (direction == ObjectAction.MovementDirection.Up) ? highlightedColor : nonHightlightedColor;
		}
	}

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

		selectedItemSection.SetActive(false);

        SetActiveTab(actionTabButton);

		leftArrow.onClick.AddListener(LeftArrow_OnClick);
		rightArrow.onClick.AddListener(RightArrow_OnClick);
		downArrow.onClick.AddListener(DownArrow_OnClick);
		upArrow.onClick.AddListener(UpArrow_OnClick);

        actionTabButton.onClick.AddListener(ActionTabButton_OnClick);
        accessoryTabButton.onClick.AddListener(AccessoryTabButton_OnClick);
        propertyTabButton.onClick.AddListener(PropertyTabButton_OnClick);

        editor.LevelEditor.OnSelectionChanged += Editor_OnSelectionChanged;
	}

	void OnDestroy()
	{
		leftArrow.onClick.RemoveListener(LeftArrow_OnClick);
		rightArrow.onClick.RemoveListener(RightArrow_OnClick);
		downArrow.onClick.RemoveListener(DownArrow_OnClick);
		upArrow.onClick.RemoveListener(UpArrow_OnClick);

        actionTabButton.onClick.RemoveListener(ActionTabButton_OnClick);
        accessoryTabButton.onClick.RemoveListener(AccessoryTabButton_OnClick);
        propertyTabButton.onClick.RemoveListener(PropertyTabButton_OnClick);

        if (editor != null)
			editor.LevelEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
	}

	private void OnPersonSelected()
	{
		var person = Person;

		lockListeners = true;
		Direction = person.InitialDirection;
		lockListeners = false;

        if (person == GlobalData.player)
        {
            SetActiveTab(accessoryTabButton);
            actionTabButton.gameObject.SetActive(false);
        }
        else
        {
            actionTabButton.gameObject.SetActive(true);
        }
	}

	private void LeftArrow_OnClick()
	{
		Direction = ObjectAction.MovementDirection.Left;
	}
	private void RightArrow_OnClick()
	{
		Direction = ObjectAction.MovementDirection.Right;
	}
	private void DownArrow_OnClick()
	{
		Direction = ObjectAction.MovementDirection.Down;
	}
	private void UpArrow_OnClick()
	{
		Direction = ObjectAction.MovementDirection.Up;
	}

    private void ActionTabButton_OnClick()
    {
        SetActiveTab(actionTabButton);
    }
    private void AccessoryTabButton_OnClick()
    {
        SetActiveTab(accessoryTabButton);
    }
    private void PropertyTabButton_OnClick()
    {
        SetActiveTab(propertyTabButton);
    }

    private void SetActiveTab(Button tabButton)
    {
        actionTabButton.interactable = tabButton != actionTabButton;
        accessoryTabButton.interactable = tabButton != accessoryTabButton;
        propertyTabButton.interactable = tabButton != propertyTabButton;

        actionSection.SetActive(tabButton == actionTabButton);
        accessorySection.SetActive(tabButton == accessoryTabButton);
        propertySection.SetActive(tabButton == propertyTabButton);
    }

    private void Editor_OnSelectionChanged(DWPObject previousItem, DWPObject newItem)
	{
		if (lockListeners)
			return;

		var person = Person;

		selectedItemSection.SetActive(person != null);

		if (person != null)
			OnPersonSelected();
	}
}