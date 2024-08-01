using UnityEngine;

public class PhobiaPersonWantsToTalkToYou : Phobia
{
    public override float DrainRate
    {
        get
        {
            return 10.0f;
        }
    }

    public override string Message
    {
        get
        {
            return "Person wants to talk to you";
        }
    }

    public PhobiaPersonWantsToTalkToYou(Player player, Person person) : base(player, person)
    {
    }
}