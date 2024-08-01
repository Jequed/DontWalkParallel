public class PhobiaPhysicalContact : Phobia
{
	public override float DrainRate
	{
		get
		{
			return 2.0f;
		}
	}

	public override string Message
	{
		get
		{
			return "Physical Contact";
		}
	}

	public PhobiaPhysicalContact(Player player, Person person) : base(player, person)
	{
	}
}