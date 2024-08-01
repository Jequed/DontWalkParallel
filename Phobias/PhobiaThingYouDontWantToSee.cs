using UnityEngine;

public class PhobiaThingYouDontWantToSee : Phobia
{
    private float drainRate;

    public override float DrainRate
    {
        get
        {
            return drainRate;
        }
    }

    public override string Message
    {
        get
        {
            return "Thing you don't want to see";
        }
    }

    public PhobiaThingYouDontWantToSee(Player player, Person person, float drainRate) : base(player, person)
    {
        this.drainRate = drainRate;
    }
}