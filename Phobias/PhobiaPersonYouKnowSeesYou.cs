using UnityEngine;

public class PhobiaPersonYouKnowSeesYou : Phobia
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
            return "Person you know sees you";
        }
    }

    public PhobiaPersonYouKnowSeesYou(Player player, Person person, float drainRate) : base(player, person)
    {
        this.drainRate = drainRate;
    }
}