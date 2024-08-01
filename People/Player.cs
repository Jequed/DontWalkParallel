using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Person
{
	private readonly KeyCode[] pressableKeys = { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow};

	private float power = 2.0f;

	private KeyCode targetKey = KeyCode.None;

	private bool isIdle = true;
	private int idleTimer = 0;
	private const int idleResetTime = 2;

	private const float maximumPhobia = 10.0f;
	private const float phobiaRecoverRate = 0.05f;
	private float phobiaLevel = 0.0f;
    private float phobiaDelta = 0.0f;
	private Queue<Phobia> currentPhobias = new Queue<Phobia>();

	public event Action<Phobia> OnPhobiaMaxed;

    public float PhobiaLevel
    {
        get
        {
            return phobiaLevel;
        }
    }
    public float MaximumPhobia
    {
        get
        {
            return maximumPhobia;
        }
    }
    public float PhobiaDelta
    {
        get
        {
            return phobiaDelta;
        }
    }

	protected override void Start()
	{
		base.Start();

		GlobalData.player = this;

		detectCollision = true;
	}

	void Update()
	{
        #if !UNITY_ANDROID
		    if (Input.GetKey(KeyCode.LeftArrow))
			    OnPressLeft();
		    else if (Input.GetKey(KeyCode.RightArrow))
			    OnPressRight();
		    else if (Input.GetKey(KeyCode.DownArrow))
			    OnPressDown();
		    else if (Input.GetKey(KeyCode.UpArrow))
			    OnPressUp();
        #endif

		if (idleTimer == 0)
		{
			isIdle = true;
		}
		else
		{
			idleTimer--;
			isIdle = false;
		}

        PlaySpeedMultiplier = 1.5f;
    }

	protected override void GameUpdate()
	{
		base.GameUpdate();

		if (!HasTarget)
		{
			if (isIdle)
			{
				switch (targetKey)
				{
                    case KeyCode.LeftArrow:
                        movementType = MovementType.idleLeft;
                        break;
                    case KeyCode.RightArrow:
                        movementType = MovementType.idleRight;
                        break;
                    case KeyCode.DownArrow:
                        movementType = MovementType.idleDown;
                        break;
                    case KeyCode.UpArrow:
                        movementType = MovementType.idleUp;
                        break;
                    case KeyCode.None:
                        break;
                }
			}
		}
		else
		{
			switch (targetKey)
			{
                case KeyCode.LeftArrow:
                    movementType = MovementType.walkLeft;
                    break;
                case KeyCode.RightArrow:
                    movementType = MovementType.walkRight;
                    break;
                case KeyCode.DownArrow:
                    movementType = MovementType.walkDown;
                    break;
                case KeyCode.UpArrow:
                    movementType = MovementType.walkUp;
                    break;
                case KeyCode.None:
                    break;
            }
		}

        UpdatePhobia();
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
		Person person = collider.GetComponentInParent<Person>();

		if (collider.name == "PhysCon")
			AddPhobia(new PhobiaPhysicalContact(this, person));
	}

	public void AddPhobia(Phobia phobia)
	{
		currentPhobias.Enqueue(phobia);
	}

	public void OnPressLeft()
	{
		ProcessArrowKey(KeyCode.LeftArrow, Vector3.left);
	}
	public void OnPressRight()
	{
		ProcessArrowKey(KeyCode.RightArrow, Vector3.right);
	}
	public void OnPressDown()
	{
		ProcessArrowKey(KeyCode.DownArrow, Vector3.down);
	}
	public void OnPressUp()
	{
		ProcessArrowKey(KeyCode.UpArrow, Vector3.up);
	}

	private void ProcessArrowKey(KeyCode keyCode, Vector3 direction)
	{
		if (!HasTarget)
		{
            //If facing the same direction
            //if (Vector3.Dot(DirectionVector, direction) > 0.9f)
            //{
                SetTarget(SnapPointToGrid(transform.position + direction), power);
            //}

			targetKey = keyCode;
		}

		idleTimer = idleResetTime;
	}

    private void UpdatePhobia()
    {
        float lastPhobiaLevel = phobiaLevel;
        Phobia mostDrainingPhobia = null;
        float mostDrainingPhobiaRate = 0.0f;
        while (currentPhobias.Count > 0)
        {
            Phobia phobia = currentPhobias.Dequeue();

            float drainRate = phobia.DrainRate;
            if (drainRate > mostDrainingPhobiaRate)
            {
                mostDrainingPhobia = phobia;
                mostDrainingPhobiaRate = drainRate;
            }
        }

        if (mostDrainingPhobia != null)
        {
            phobiaLevel += mostDrainingPhobiaRate * GlobalData.phobiaTimeMultiplier;

            if (phobiaLevel > maximumPhobia)
                OnPhobiaMaxedInternal(mostDrainingPhobia);
        }

        phobiaLevel -= phobiaRecoverRate;
        if (phobiaLevel < 0.0f)
            phobiaLevel = 0.0f;

        phobiaDelta = phobiaLevel - lastPhobiaLevel;
    }

	private void OnPhobiaMaxedInternal(Phobia phobia)
	{
		if (OnPhobiaMaxed != null)
			OnPhobiaMaxed(phobia);
	}
}