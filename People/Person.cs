using System.Collections.Generic;
using UnityEngine;

public class Person : WorldObject
{
    public enum BehaviorType
    {
        neutral = 0,
        texting = 1
    }

    public enum MovementType
    {
        none = 0,

        idleLeft = 1,
        idleRight = 2,
        idleDown = 3,
        idleUp = 4,

        walkLeft = 5,
        walkRight = 6,
        walkDown = 7,
        walkUp = 8,
    }

    public enum Gender
    {
        neutral = 0,
        male = 1,
        female = 2
    }

    [SerializeField]
    protected ObjectAction.MovementDirection initialDirection = ObjectAction.MovementDirection.Down;

    public Gender gender = Gender.male;

    protected BehaviorType behaviorType = BehaviorType.neutral;
    protected MovementType movementType = MovementType.idleDown;

    protected PersonAppearanceManager appearanceManager;

	protected Rigidbody rigidBody;

	protected bool detectCollision = false;

	private Vector3 previousPosition;

	private Vector3 velocity;

	private Vector3 targetPosition;
	private float targetSpeed = 0.0f;
	private bool hasTarget = false;

	private const float size = 1.0f;

    public PersonAppearanceManager AppearanceManager
    {
        get
        {
            return appearanceManager;
        }
    }

    public ObjectAction.MovementDirection Direction
    {
        get
        {
            switch (movementType)
            {
                case MovementType.idleLeft:
                case MovementType.walkLeft:
                    return ObjectAction.MovementDirection.Left;
                case MovementType.idleRight:
                case MovementType.walkRight:
                    return ObjectAction.MovementDirection.Right;
                case MovementType.idleUp:
                case MovementType.walkUp:
                    return ObjectAction.MovementDirection.Up;
                case MovementType.idleDown:
                case MovementType.walkDown:
                    return ObjectAction.MovementDirection.Down;
                default:
                    break;
            }

            return ObjectAction.MovementDirection.None;
        }
    }

    public Vector3 DirectionVector
    {
        get
        {
            switch (movementType)
            {
                case MovementType.idleLeft:
                case MovementType.walkLeft:
                    return Vector3.left;
                case MovementType.idleRight:
                case MovementType.walkRight:
                    return Vector3.right;
                case MovementType.idleDown:
                case MovementType.walkDown:
                    return Vector3.down;
                case MovementType.idleUp:
                case MovementType.walkUp:
                    return Vector3.up;
                default:
                case MovementType.none:
                    return Vector3.zero;
            }
        }
    }

	public bool HasTarget
	{
		get
		{
			return hasTarget;
		}
	}

    public ObjectAction.MovementDirection InitialDirection
    {
        get
        {
            return initialDirection;
        }
        set
        {
            initialDirection = value;

            switch (value)
            {
                case ObjectAction.MovementDirection.Left:
                    SetMovementType(Player.MovementType.idleLeft);
                    break;
                case ObjectAction.MovementDirection.Right:
                    SetMovementType(Player.MovementType.idleRight);
                    break;
                default:
                case ObjectAction.MovementDirection.Down:
                    SetMovementType(Player.MovementType.idleDown);
                    break;
                case ObjectAction.MovementDirection.Up:
                    SetMovementType(Player.MovementType.idleUp);
                    break;
            }
        }
    }

    public MovementType CurrentMovementType
    {
        get
        {
            return movementType;
        }
    }

    public PersonProperty[] Properties
    {
        get
        {
            return GetComponentsInChildren<PersonProperty>();
        }
    }

    public static float Size
	{
		get
		{
			return size;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return velocity;
		}
	}

    protected override void Awake()
    {
        base.Awake();

        appearanceManager = new PersonAppearanceManager(this);
    }

    protected override void Start()
	{
		base.Start();

        rigidBody = GetComponent<Rigidbody>();

		previousPosition = transform.position;
	}

    protected override void EditorUpdate()
    {
        base.EditorUpdate();

        UpdateAppearance();
    }

    protected override void GameUpdate()
	{
		base.GameUpdate();

		velocity = transform.position - previousPosition;

        UpdateAppearance();

		UpdateTarget();

		previousPosition = transform.position;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (PlayMode)
			transform.position = SnapPointToGrid(transform.position);
	}

	public void SetTarget(Vector3 position, float speed)
	{
		targetPosition = position;
		targetSpeed = speed;
		hasTarget = true;
	}

	protected virtual void OnTargetEnd(Vector3 targetPosition, Vector3 direction) { }

    private void UpdateAppearance()
    {
        foreach (var animation in PersonData.personAnimations)
        {
            if (animation.behavior == behaviorType && animation.movement == movementType)
            {
                SetImageSeries(animation.imageSeries);
                break;
            }
        }

        appearanceManager.UpdateImages(behaviorType, movementType);
    }

	private void UpdateTarget()
	{
		if (hasTarget)
		{
			Vector3 moveVector = (targetPosition - transform.position).normalized;

			bool hitCollider = false;

			if (detectCollision)
			{
				Vector3 dir = targetPosition - transform.position;
				RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position, dir.normalized), dir.magnitude);

				foreach (var hit in hits)
				{
					if (hit.collider != null && !hit.collider.isTrigger)
					{
						RemoveTarget();
						hitCollider = true;
						break;
					}
				}

				if (!hitCollider)
				{
					if (Vector3.Distance(transform.position, targetPosition) < targetSpeed * Time.fixedDeltaTime)
					{
						transform.position = targetPosition;
						OnTargetEnd(targetPosition, moveVector);
						RemoveTarget();
						hitCollider = true;
					}
				}
			}

			if (!hitCollider)
				rigidBody.velocity = moveVector * targetSpeed * GlobalData.timeMultiplier;
		}
	}

	private void RemoveTarget()
	{
		rigidBody.velocity = Vector3.zero;
		hasTarget = false;
	}

	private void OnFlashing()
	{

	}

    public void SetMovementType(MovementType type)
    {
        movementType = type;
    }

    public void RandomizeAppearance()
    {
        appearanceManager.Randomize();

        UpdateAppearance();
    }

    public PersonProperty AddProperty(PersonProperty.Type type)
    {
        return AddProperty(type.ToString());
    }
    public PersonProperty AddProperty(string propertyType)
    {
        var property = Instantiate(Resources.Load<PersonProperty>("Prefabs/GamePrefabs/PersonProperties/" + propertyType));
        property.transform.parent = transform;
        property.transform.localPosition = Vector3.zero;

        return property;
    }

    public PersonProperty GetProperty(PersonProperty.Type type)
    {
        var properties = Properties;

        foreach (var property in properties)
        {
            if (property.PropertyType == type)
                return property;
        }

        return null;
    }

    public virtual void ProcessPeople(Person[] people) { }
}