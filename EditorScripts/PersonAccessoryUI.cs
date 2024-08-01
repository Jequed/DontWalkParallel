using System;
using UnityEngine;
using UnityEngine.UI;

public class PersonAccessoryUI : MonoBehaviour
{
    public event Action<PersonAccessoryUI> OnOrderArrowUpClicked;
    public event Action<PersonAccessoryUI> OnOrderArrowDownClicked;

    public event Action<PersonAccessoryUI> OnDeleteButtonClicked;

    [SerializeField]
    private PersonAccessoryUITitle title;

    [SerializeField]
    private GameObject optionsArea;

    private LayoutElement layoutElement;

    private PersonAccessoryInstance accessory;

    private readonly Color highlightedColor = Color.blue;
    private readonly Color nonHightlightedColor = Color.white;

    public PersonAccessoryInstance AccessoryInstance
    {
        get
        {
            return accessory;
        }
        set
        {
            accessory = value;

            UpdateTitleText();

            Expanded = value.expanded;
        }
    }

    public bool Expanded
    {
        get
        {
            return accessory.expanded;
        }
        set
        {
            accessory.expanded = value;

            title.expandArrowExpanded.SetActive(value);
            title.expandArrowNotExpanded.SetActive(!value);

            optionsArea.gameObject.SetActive(value);

            UpdateLayoutHeight();
        }
    }

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();

        title.expandArrow.onClick.AddListener(ExpandArrow_OnClick);

        title.orderArrowUp.onClick.AddListener(OrderArrowUp_OnClick);
        title.orderArrowDown.onClick.AddListener(OrderArrowDown_OnClick);

        title.deleteButton.onClick.AddListener(DeleteButton_OnClick);
    }

    void OnDestroy()
    {
        title.expandArrow.onClick.RemoveListener(ExpandArrow_OnClick);

        title.orderArrowUp.onClick.RemoveListener(OrderArrowUp_OnClick);
        title.orderArrowDown.onClick.RemoveListener(OrderArrowDown_OnClick);

        title.deleteButton.onClick.RemoveListener(DeleteButton_OnClick);
    }

    private void UpdateLayoutHeight()
    {
        float layoutHeight = 1.0f;

        if (accessory.expanded)
            layoutHeight = 100.0f;
        else
            layoutHeight = 50.0f;

        layoutElement.minHeight = layoutHeight;
    }

    private void UpdateTitleText()
    {
        title.accessoryText.text = PersonData.GetAccessoryFromData(accessory.accessoryData).name;
    }

    private void ExpandArrow_OnClick()
    {
        Expanded = !Expanded;
    }

    private void OrderArrowUp_OnClick()
    {
        OnOrderArrowUpClicked(this);
    }
    private void OrderArrowDown_OnClick()
    {
        OnOrderArrowDownClicked(this);
    }

    private void DeleteButton_OnClick()
    {
        OnDeleteButtonClicked(this);
    }
}