using System.Collections.Generic;
using UnityEngine;

public class NPCActionController
{
	private NPC npc;

	private float speed = 1.0f;

    private Person.MovementType startingMovementType;

    private bool repeat = true;

	private bool startAutomatically = true;

	private List<ObjectAction> actions;

	private Dictionary<ObjectAction, float> actionDuration = new Dictionary<ObjectAction, float>();

    private ObjectAction lastAction = null;

	private float totalDuration;

	private float elapsedTime;

	private Vector3 startingPosition;

    private Person.MovementType lastMovementType;

    private bool started = false;

    private bool paused = false;

    public List<ObjectAction> Actions
    {
        get
        {
            return actions;
        }
    }

    public float ElapsedTime
    {
        get
        {
            return elapsedTime;
        }
        set
        {
            elapsedTime = value;
        }
    }

    public ObjectAction LastAction
    {
        get
        {
            return lastAction;
        }
    }

    public bool Paused
    {
        get
        {
            return paused;
        }
        set
        {
            paused = value;
        }
    }

    public float TotalDuration
    {
        get
        {
            return totalDuration;
        }
    }

	public NPCActionController(NPC npc, float speed, List<ObjectAction> actions, Person.MovementType startingMovementType)
	{
		this.npc = npc;
		this.speed = speed;
        this.startingMovementType = startingMovementType;
		this.actions = actions;

		startingPosition = npc.transform.position;

		if (startAutomatically)
			BeginActions();

        lastMovementType = startingMovementType;
    }

	public void FixedUpdate()
	{
		if (started && actions.Count > 0)
		{
			Vector3 beginPosition = npc.transform.position;

            if (!paused)
			    elapsedTime += Time.fixedDeltaTime * GlobalData.timeMultiplier;

			float currentTime = repeat ? Mathf.Repeat(elapsedTime, totalDuration) : elapsedTime;

			float leftOverTime = currentTime;
			Vector3 currentPosition = startingPosition;
			lastAction = actions[0];
			for (int i = 0; i <actions.Count; i++)
			{
                var action = actions[i];
				if (actionDuration[action] > leftOverTime)
				{
					if (action.wait)
						npc.transform.position = currentPosition;
					else
						npc.transform.position = currentPosition + action.DirectionVector * action.distance * (leftOverTime / actionDuration[action]);
					
					lastAction = action;
					break;
				}
				else
				{
					currentPosition += action.DirectionVector * action.distance;
					leftOverTime -= actionDuration[action];

                    if (action.playOnce)
                    {
                        var d = actionDuration[action];
                        actionDuration[action] = 0.0f;
                        totalDuration -= d;
                        elapsedTime -= d;
                        //leftOverTime -= d;

                        //actions.Remove(action);
                        //if (i > 0)
                        //    i--;

                        //CalculateTotalDuration();
                    }
				}
			}


			if (!repeat && elapsedTime > totalDuration)
				lastAction = actions[actions.Count - 1];

            Person.MovementType movementType = lastMovementType;

            Vector3 delta = npc.transform.position - beginPosition;

            if (lastAction.wait || paused)
            {
                switch (lastAction.direction)
                {
                    case ObjectAction.MovementDirection.Left:
                        movementType = Person.MovementType.idleLeft;
                        break;
                    case ObjectAction.MovementDirection.Right:
                        movementType = Person.MovementType.idleRight;
                        break;
                    case ObjectAction.MovementDirection.Down:
                        movementType = Person.MovementType.idleDown;
                        break;
                    case ObjectAction.MovementDirection.Up:
                        movementType = Person.MovementType.idleUp;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (lastAction.direction)
                {
                    case ObjectAction.MovementDirection.Left:
                        movementType = Person.MovementType.walkLeft;
                        break;
                    case ObjectAction.MovementDirection.Right:
                        movementType = Person.MovementType.walkRight;
                        break;
                    case ObjectAction.MovementDirection.Down:
                        movementType = Person.MovementType.walkDown;
                        break;
                    case ObjectAction.MovementDirection.Up:
                        movementType = Person.MovementType.walkUp;
                        break;
                    default:
                        break;
                }
            }

            npc.PlaySpeedMultiplier = speed * lastAction.speedMultiplier * 1.5f;
            npc.SetMovementType(movementType);

            lastMovementType = movementType;
        }
		else
		{
			//worldObject.SetImageSeries(startingTag);
		}
	}

	public void BeginActions()
	{
        CalculateTotalDuration();
        npc.transform.position = startingPosition;
		elapsedTime = 0.0f;
		started = true;
	}

    private void CalculateTotalDuration()
    {
        actionDuration.Clear();
        totalDuration = 0.0f;
        foreach (var action in actions)
        {
            float duration = 1.0f;

            if (action.wait)
                duration = action.waitTime;
            else
                duration = action.distance / (speed * action.speedMultiplier);

            actionDuration.Add(action, duration);
            totalDuration += duration;
        }
    }

	private class MovementPatterAction
	{
		public Vector3 displacement;
		public float duration;

		public MovementPatterAction(Vector3 displacement, float speed)
		{
			this.displacement = displacement;

			duration = displacement.magnitude / speed;
		}

		public MovementPatterAction(float duration)
		{
			displacement = Vector3.zero;
			this.duration = duration;
		}
	}
}