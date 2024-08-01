public abstract class Phobia
{
	protected Player player;
	protected Person person;

	public Person Person
	{
		get
		{
			return person;
		}
	}

	public abstract float DrainRate { get; }

	public abstract string Message { get; }
	
	public Phobia(Player player, Person person)
	{
		this.player = player;
		this.person = person;
	}
}