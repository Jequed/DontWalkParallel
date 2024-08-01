using System;
using UnityEngine;
using UnityEngine.UI;

public class PersonPropertyUI : MonoBehaviour
{
    public event Action<PersonPropertyUI> OnOrderArrowUpClicked;
    public event Action<PersonPropertyUI> OnOrderArrowDownClicked;

    public event Action<PersonPropertyUI> OnDeleteButtonClicked;

    [SerializeField]
    private PersonPropertyUITitle title;

    [SerializeField]
    private GameObject optionsArea;

    private LayoutElement layoutElement;

    private PersonProperty property = null;

    private GameObject propertyUI;

    private readonly Color highlightedColor = Color.blue;
    private readonly Color nonHightlightedColor = Color.white;

    public PersonProperty Property
    {
        get
        {
            return property;
        }
        set
        {
            if (property != value)
            {
                if (property != null)
                    Destroy(propertyUI);

                property = value;

                if (property != null)
                {
                    propertyUI = Instantiate(Resources.Load<GameObject>("Prefabs/EditorPrefabs/Properties/" + property.PropertyType.ToString() + "UI"));
                    propertyUI.transform.parent = optionsArea.transform;
                }

                UpdateTitleText();

                Expanded = value.expanded;
            }
        }
    }

    public bool Expanded
    {
        get
        {
            return property.expanded;
        }
        set
        {
            property.expanded = value;

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

        if (property.expanded)
            layoutHeight = 50.0f + propertyUI.GetComponent<RectTransform>().rect.height;
        else
            layoutHeight = 50.0f;

        layoutElement.minHeight = layoutHeight;
    }

    private void UpdateTitleText()
    {
        title.propertyText.text = property.PropertyType.ToString();
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