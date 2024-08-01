using UnityEngine;

[System.Serializable]
public class ObjectAction
{
	public bool expanded = true;

	public bool wait = false;

	public MovementDirection direction = MovementDirection.Down;

	public float distance = 0.0f;

	public float waitTime = 0.0f;

	public float speedMultiplier = 1.0f;

    public bool playOnce = false;

	public bool overrideAnimation = false;

	public enum MovementDirection
	{
		None,
		Left,
		Right,
		Down,
		Up
	}

	public Vector3 DirectionVector
	{
		get
		{
			switch (direction)
			{
				case MovementDirection.None:
					return Vector3.zero;
				case MovementDirection.Left:
					return Vector3.left;
				case MovementDirection.Right:
					return Vector3.right;
				case MovementDirection.Down:
					return Vector3.down;
				case MovementDirection.Up:
					return Vector3.up;
			}

			return Vector3.zero;
		}
	}

    public static MovementDirection OppositeDirection(MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Left:
                return MovementDirection.Right;
            case MovementDirection.Right:
                return MovementDirection.Left;
            case MovementDirection.Up:
                return MovementDirection.Down;
            case MovementDirection.Down:
                return MovementDirection.Up;
            default:
                return MovementDirection.None;
        }
    }
}