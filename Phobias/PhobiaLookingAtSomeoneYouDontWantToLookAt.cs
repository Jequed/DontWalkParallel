using UnityEngine;

public class PhobiaLookingAtSomeoneYouDontWantToLookAt : Phobia
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
            return "Looking at someone you don't want to look at";
        }
    }

    public PhobiaLookingAtSomeoneYouDontWantToLookAt(Player player, Person person) : base(player, person)
    {
    }
}