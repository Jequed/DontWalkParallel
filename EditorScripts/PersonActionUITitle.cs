using UnityEngine;
using UnityEngine.UI;

public class PersonActionUITitle : MonoBehaviour
{
	[SerializeField]
	public Button expandArrow;

	[SerializeField]
	public GameObject expandArrowExpanded;

	[SerializeField]
	public GameObject expandArrowNotExpanded;

	[SerializeField]
	public GameObject waitLeft;

	[SerializeField]
	public GameObject waitRight;

	[SerializeField]
	public GameObject waitDown;

	[SerializeField]
	public GameObject waitUp;

	[SerializeField]
	public GameObject moveLeft;

	[SerializeField]
	public GameObject moveRight;

	[SerializeField]
	public GameObject moveDown;

	[SerializeField]
	public GameObject moveUp;

	[SerializeField]
	public Text actionText;

	[SerializeField]
	public Button orderArrowUp;

	[SerializeField]
	public Button orderArrowDown;

	[SerializeField]
	public Button deleteButton;

	public ObjectAction.MovementDirection WaitDirection
	{
		set
		{
			waitLeft.SetActive(value == ObjectAction.MovementDirection.Left);
			waitRight.SetActive(value == ObjectAction.MovementDirection.Right);
			waitDown.SetActive(value == ObjectAction.MovementDirection.Down);
			waitUp.SetActive(value == ObjectAction.MovementDirection.Up);
			moveLeft.SetActive(false);
			moveRight.SetActive(false);
			moveDown.SetActive(false);
			moveUp.SetActive(false);
		}
	}

	public ObjectAction.MovementDirection MoveDirection
	{
		set
		{
			moveLeft.SetActive(value == ObjectAction.MovementDirection.Left);
			moveRight.SetActive(value == ObjectAction.MovementDirection.Right);
			moveDown.SetActive(value == ObjectAction.MovementDirection.Down);
			moveUp.SetActive(value == ObjectAction.MovementDirection.Up);
			waitLeft.SetActive(false);
			waitRight.SetActive(false);
			waitDown.SetActive(false);
			waitUp.SetActive(false);
		}
	}
}