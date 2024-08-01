public class PhobiaBadTile : Phobia
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
            return "Bad tile";
        }
    }

    public PhobiaBadTile(Player player, Person person) : base(player, person)
    {
    }
}