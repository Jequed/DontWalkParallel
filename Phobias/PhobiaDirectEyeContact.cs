using UnityEngine;

public class PhobiaDirectEyeContact : Phobia
{
	public override float DrainRate
	{
		get
		{
			float distance = Vector3.Distance(player.transform.position, person.transform.position);
			if (distance == 0.0f)
				return 10.0f;

			return 1.0f / Vector3.Distance(player.transform.position, person.transform.position);
		}
	}

	public override string Message
	{
		get
		{
			return "Direct Eye Contact";
		}
	}

	public PhobiaDirectEyeContact(Player player, Person person) : base(player, person)
	{
	}
}