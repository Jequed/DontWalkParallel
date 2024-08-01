using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonActionUI : MonoBehaviour
{
	public event Action<PersonActionUI> OnOrderArrowUpClicked;
	public event Action<PersonActionUI> OnOrderArrowDownClicked;

	public event Action<PersonActionUI> OnDeleteButtonClicked;

	public event Action OnPathChanged;

	[SerializeField]
	private PersonActionUITitle title;

	[SerializeField]
	private GameObject optionsArea;

	[SerializeField]
	private Button waitButton;

	[SerializeField]
	private Button moveButton;

	[SerializeField]
	private Button leftArrow;

	[SerializeField]
	private Button rightArrow;

	[SerializeField]
	private Button downArrow;

	[SerializeField]
	private Button upArrow;

	[SerializeField]
	private InputField waitInputField;

	[SerializeField]
	private InputField distanceInputField;

	[SerializeField]
	private InputField speedInputField;

	[SerializeField]
	private Toggle overrideAnimationToggle;

	[SerializeField]
	private GameObject overrideAnimationSection;

	[SerializeField]
	private Dropdown overrideActionDropdown;

	private LayoutElement layoutElement;

	private ObjectAction action;

	private readonly Color highlightedColor = Color.blue;
	private readonly Color nonHightlightedColor = Color.white;

	public ObjectAction Action
	{
		get
		{
			return action;
		}
		set
		{
			action = value;

			Direction = value.direction;

			Expanded = value.expanded;

			Wait = value.wait;

			waitInputField.transform.parent.gameObject.SetActive(action.wait);
			distanceInputField.transform.parent.gameObject.SetActive(!action.wait);
			speedInputField.transform.parent.gameObject.SetActive(!action.wait);

			overrideAnimationSection.SetActive(action.overrideAnimation);
		}
	}

	public ObjectAction.MovementDirection Direction
	{
		get
		{
			return action.direction;
		}
		set
		{
			action.direction = value;

			leftArrow.targetGraphic.color = (action.direction == ObjectAction.MovementDirection.Left) ? highlightedColor : nonHightlightedColor;
			rightArrow.targetGraphic.color = (action.direction == ObjectAction.MovementDirection.Right) ? highlightedColor : nonHightlightedColor;
			downArrow.targetGraphic.color = (action.direction == ObjectAction.MovementDirection.Down) ? highlightedColor : nonHightlightedColor;
			upArrow.targetGraphic.color = (action.direction == ObjectAction.MovementDirection.Up) ? highlightedColor : nonHightlightedColor;

			if (action.wait)
				title.WaitDirection = value;
			else
				title.MoveDirection = value;

            if (OnPathChanged != null)
                OnPathChanged();
		}
	}

	public bool Expanded
	{
		get
		{
			return action.expanded;
		}
		set
		{
			action.expanded = value;

			title.expandArrowExpanded.SetActive(value);
			title.expandArrowNotExpanded.SetActive(!value);

			optionsArea.gameObject.SetActive(value);

			UpdateLayoutHeight();
		}
	}

	public bool Wait
	{
		get
		{
			return action.wait;
		}
		set
		{
			action.wait = value;

			waitButton.interactable = !value;
			moveButton.interactable = value;

			waitInputField.transform.parent.gameObject.SetActive(value);
			distanceInputField.transform.parent.gameObject.SetActive(!value);
			speedInputField.transform.parent.gameObject.SetActive(!value);

			waitInputField.text = action.waitTime.ToString();
			distanceInputField.text = action.distance.ToString();
			speedInputField.text = action.speedMultiplier.ToString();

			Direction = action.direction;

			UpdateTitleText();
		}
	}

	void Awake()
	{
		layoutElement = GetComponent<LayoutElement>();

		title.expandArrow.onClick.AddListener(ExpandArrow_OnClick);

		title.orderArrowUp.onClick.AddListener(OrderArrowUp_OnClick);
		title.orderArrowDown.onClick.AddListener(OrderArrowDown_OnClick);

		title.deleteButton.onClick.AddListener(DeleteButton_OnClick);

		waitButton.onClick.AddListener(WaitButton_OnClick);
		moveButton.onClick.AddListener(MoveButton_OnClick);

		leftArrow.onClick.AddListener(LeftArrow_OnClick);
		rightArrow.onClick.AddListener(RightArrow_OnClick);
		downArrow.onClick.AddListener(DownArrow_OnClick);
		upArrow.onClick.AddListener(UpArrow_OnClick);

		waitInputField.onValueChanged.AddListener(WaitInputField_OnValueChanged);
		distanceInputField.onValueChanged.AddListener(DistanceInputField_OnValueChanged);
		speedInputField.onValueChanged.AddListener(SpeedInputField_OnValueChanged);

		overrideAnimationToggle.onValueChanged.AddListener(OverrideAnimationToggle_OnValueChanged);

		var options = new List<string>();
		//foreach (var tag in Enum.GetValues(typeof(ImageSeries.Tag)))
		//	options.Add(tag.ToString());
		//options[0] = "automatic";
		//overrideActionDropdown.AddOptions(options);
	}

	void OnDestroy()
	{
		title.expandArrow.onClick.RemoveListener(ExpandArrow_OnClick);

		title.orderArrowUp.onClick.RemoveListener(OrderArrowUp_OnClick);
		title.orderArrowDown.onClick.RemoveListener(OrderArrowDown_OnClick);

		title.deleteButton.onClick.RemoveListener(DeleteButton_OnClick);

		waitButton.onClick.RemoveListener(WaitButton_OnClick);
		moveButton.onClick.RemoveListener(MoveButton_OnClick);

		leftArrow.onClick.RemoveListener(LeftArrow_OnClick);
		rightArrow.onClick.RemoveListener(RightArrow_OnClick);
		downArrow.onClick.RemoveListener(DownArrow_OnClick);
		upArrow.onClick.RemoveListener(UpArrow_OnClick);

		waitInputField.onValueChanged.RemoveListener(WaitInputField_OnValueChanged);
		distanceInputField.onValueChanged.RemoveListener(DistanceInputField_OnValueChanged);
		speedInputField.onValueChanged.RemoveListener(SpeedInputField_OnValueChanged);

		overrideAnimationToggle.onValueChanged.RemoveListener(OverrideAnimationToggle_OnValueChanged);
	}

	private void UpdateLayoutHeight()
	{
		float layoutHeight = 1.0f;

		if (action.expanded)
		{
			layoutHeight = 450.0f;

			if (action.overrideAnimation)
				layoutHeight += 50.0f;
		}
		else
		{
			layoutHeight = 50.0f;
		}

		layoutElement.minHeight = layoutHeight;
	}

	private void UpdateTitleText()
	{
		if (action.wait)
		{
			title.actionText.text = action.waitTime.ToString();
		}
		else
		{
			title.actionText.text = action.distance.ToString();
			if (Mathf.Abs(action.speedMultiplier - 1.0f) > 0.01f)
				title.actionText.text += " (x" + action.speedMultiplier + ")";
		}
	}

	private void ExpandArrow_OnClick()
	{
		Expanded = !Expanded;
	}

	private void OrderArrowUp_OnClick()
	{
		OnOrderArrowUpClicked(this);
		OnPathChanged();
	}
	private void OrderArrowDown_OnClick()
	{
		OnOrderArrowDownClicked(this);
        if (OnPathChanged != null)
            OnPathChanged();
	}

	private void DeleteButton_OnClick()
	{
		OnDeleteButtonClicked(this);
        if (OnPathChanged != null)
		    OnPathChanged();
	}

	private void WaitButton_OnClick()
	{
		Wait = true;
	}
	private void MoveButton_OnClick()
	{
		Wait = false;
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

	private void WaitInputField_OnValueChanged(string value)
	{
		action.waitTime = float.Parse(value);
		UpdateTitleText();
        if (OnPathChanged != null)
            OnPathChanged();
    }
	private void DistanceInputField_OnValueChanged(string value)
	{
		try
		{
			action.distance = float.Parse(value);
			UpdateTitleText();
            if (OnPathChanged != null)
                OnPathChanged();
		}
		catch (FormatException)
		{
		}
	}
	private void SpeedInputField_OnValueChanged(string value)
	{
		try
		{
			action.speedMultiplier = float.Parse(value);
			UpdateTitleText();
            if (OnPathChanged != null)
                OnPathChanged();
        }
		catch (FormatException)
		{
		}
	}

	private void OverrideAnimationToggle_OnValueChanged(bool value)
	{
		action.overrideAnimation = value;

		overrideAnimationSection.SetActive(value);

		UpdateLayoutHeight();
	}
}