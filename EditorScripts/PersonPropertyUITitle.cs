using UnityEngine;
using UnityEngine.UI;

public class PersonPropertyUITitle : MonoBehaviour
{
    [SerializeField]
    public Button expandArrow;

    [SerializeField]
    public GameObject expandArrowExpanded;

    [SerializeField]
    public GameObject expandArrowNotExpanded;

    [SerializeField]
    public Text propertyText;

    [SerializeField]
    public Button orderArrowUp;

    [SerializeField]
    public Button orderArrowDown;

    [SerializeField]
    public Button deleteButton;
}