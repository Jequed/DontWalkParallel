using System.Collections.Generic;
using UnityEngine;

public class NPC : Person
{
	[SerializeField]
	private List<ObjectAction> actions = new List<ObjectAction>();

	private NPCActionController actionController = null;

    public const float speed = 1.0f;

	public List<ObjectAction> Actions
	{
		get
		{
			return actions;
		}
        set
        {
            actions = value;
        }
	}

    public NPCActionController ActionController
    {
        get
        {
            return actionController;
        }
        set
        {
            actionController = value;
        }
    }

	protected override void Start()
	{
		base.Start();

		InitialDirection = initialDirection;
	}

	protected override void GameAwake()
	{
        actionController = new NPCActionController(this, speed, actions, MovementType.idleDown);
    }

	protected override void GameUpdate()
	{
		base.GameUpdate();

		if (actionController != null)
			actionController.FixedUpdate();
	}
}