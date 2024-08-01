using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public event Action<Phobia> OnPhobiaMaxed;

	private const float epsilon = 0.0001f;

	private Player player;

	private List<NPC> visiblePeople = new List<NPC>();

	void Start()
	{
		player = GetComponentInChildren<Player>();
		if (player != null)
			player.OnPhobiaMaxed += Player_OnPhobiaMaxed;
	}

	void OnDestroy()
	{
		if (player != null)
			player.OnPhobiaMaxed -= Player_OnPhobiaMaxed;
	}

	void FixedUpdate()
	{
		if (GlobalData.playMode && player != null)
		{
            if (GlobalData.player.PhobiaDelta > 0.0f)
                GlobalData.timeMultiplier = GlobalData.phobiaTimeMultiplier;
            else
                GlobalData.timeMultiplier = GlobalData.defaultTimeMultiplier;

			FindVisiblePeople();

            GlobalData.allNPCs = GetComponentsInChildren<NPC>();

            foreach (var person in GlobalData.allNPCs)
				person.Flashing = false;

			// Walking parallel
			if (player.Velocity.magnitude > epsilon)
			{
				foreach (var person in visiblePeople)
				{
					if (person.Velocity.magnitude > epsilon)
					{
						// Are the people facing the same way
						if (Vector3.Dot(player.Velocity.normalized, person.Velocity.normalized) >= 1.0f - epsilon)
						{
							Vector3 projection = Vector3.Project(player.transform.position - person.transform.position, person.Velocity);

							// Are they on the same line
							if (projection.magnitude < Person.Size)
							{
								player.AddPhobia(new PhobiaWalkingParallel(player, person));
								person.Flashing = true;
							}
						}
					}
				}
			}

			// Direct eye contact
			Vector3 playerDirectionVector = player.DirectionVector;
			NPC closestPersonDirectEyeContact = null;
            foreach (var person in visiblePeople)
            {
                Vector3 personDirectionVector = person.DirectionVector;

                // Are the people facing opposite directions
                if (Vector3.Dot(playerDirectionVector, personDirectionVector) <= -1.0f + epsilon)
                {
                    Vector3 sub = player.transform.position - person.transform.position;
                    Vector3 projection = Vector3.Project(sub, personDirectionVector);

                    // Are they on the same line
                    if ((sub - projection).magnitude <= Mathf.Epsilon)
                    {
                        // Are they facing away or towards each other
                        if (Vector3.Dot(sub, personDirectionVector) > 0.0f)
                        {
                            if (closestPersonDirectEyeContact != null)
                            {
                                if (Vector3.Distance(player.transform.position, person.transform.position) < Vector3.Distance(player.transform.position, closestPersonDirectEyeContact.transform.position))
                                    closestPersonDirectEyeContact = person;
                            }
                            else
                            {
                                closestPersonDirectEyeContact = person;
                            }
                        }
                    }
                }
            }
            if (closestPersonDirectEyeContact != null)
			{
				player.AddPhobia(new PhobiaDirectEyeContact(player, closestPersonDirectEyeContact));
				closestPersonDirectEyeContact.Flashing = true;
			}

            // If the player has a viewcone, use that instead
            if (!player.GetProperty(PersonProperty.Type.ViewCone))
            {
                // Don't look at this person
                NPC closestPersonDontLookAt = null;
                foreach (var person in visiblePeople)
                {
                    if (person.GetProperty(PersonProperty.Type.DontLookAt) != null)
                    {
                        Vector3 sub = player.transform.position - person.transform.position;
                        Vector3 projection = Vector3.Project(sub, playerDirectionVector);

                        // Are they on the same line
                        if ((sub - projection).magnitude <= Mathf.Epsilon)
                        {
                            // Is the player facing the person
                            if (Vector3.Dot(playerDirectionVector, sub) < 0.0f)
                            {
                                if (closestPersonDontLookAt != null)
                                {
                                    if (Vector3.Distance(player.transform.position, person.transform.position) < Vector3.Distance(player.transform.position, closestPersonDontLookAt.transform.position))
                                        closestPersonDontLookAt = person;
                                }
                                else
                                {
                                    closestPersonDontLookAt = person;
                                }
                            }
                        }
                    }
                }
                if (closestPersonDontLookAt != null)
                {
                    player.AddPhobia(new PhobiaLookingAtSomeoneYouDontWantToLookAt(player, closestPersonDontLookAt));
                    closestPersonDontLookAt.Flashing = true;
                }
            }
        }
	}
	
	private void FindVisiblePeople()
	{
		visiblePeople.Clear();

		for (int i = 0; i < 4; i++)
		{
			Vector3 rayDirection = Vector3.right;
			switch (i)
			{
				case 0:
					rayDirection = Vector3.left;
					break;
				case 1:
					rayDirection = Vector3.right;
					break;
				case 2:
					rayDirection = Vector3.down;
					break;
				case 3:
					rayDirection = Vector3.up;
					break;
			}

			var ray = new Ray(player.transform.position, rayDirection);
			RaycastHit[] hits = Physics.RaycastAll(ray);

			DWPObject closestHit = null;
			Vector3 closestHitPoint = Vector3.zero;
			foreach (var hit in hits)
			{
				DWPObject obj = null;

				if (hit.collider.name == "RayHit")
				{
					var person = hit.collider.GetComponentInParent<NPC>();

					if (person != null)
						obj = person;
				}
				else
				{
					SolidObject solidObject = hit.transform.GetComponentInChildren<SolidObject>();

					if (solidObject != null && !solidObject.canSeeOver)
						obj = solidObject;
				}

				if (obj != null)
				{
					if (closestHit == null || Vector3.Distance(ray.origin, hit.point) < Vector3.Distance(ray.origin, closestHitPoint))
					{
						closestHit = obj;
						closestHitPoint = hit.point;
					}
				}
			}

			if (closestHit != null && closestHit is NPC)
			{
				var person = closestHit as NPC;

				var viewportPoint = Camera.main.WorldToViewportPoint(person.transform.position);

				//Don't count people offscreen
				if (viewportPoint.x > 0.0f && viewportPoint.x < 1.0f && viewportPoint.y > 0.0f && viewportPoint.y < 1.0f)
					visiblePeople.Add(person);
			}

			/*DWPObject closestHit = null;
			if (closestPerson != null && closestSolidObject != null)
			{
				// If the person is on top of the solid object don't count it.
				if (Vector3.Distance(closestPersonHit, closestSolidObjectHit) < 0.5f)
				{
					closestHit = closestPerson;
				}
				else
				{
					if (Vector3.Distance(ray.origin, closestPersonHit) < 0.5f && Vector3.Distance(ray.origin, closestSolidObjectHit) < 0.5f)
						closestHit = closestPerson;
					else
						closestHit = closestSolidObject;
				}
			}
			else if (closestPerson != null)
				closestHit = closestPerson;
			else if (closestSolidObject != null)
				closestHit = closestSolidObject;

			if (closestHit != null && closestHit is NPC)
			{
				var person = closestHit as NPC;

				var viewportPoint = Camera.main.WorldToViewportPoint(person.transform.position);

				//Don't count people offscreen
				if (viewportPoint.x > 0.0f && viewportPoint.x < 1.0f && viewportPoint.y > 0.0f && viewportPoint.y < 1.0f)
					visiblePeople.Add(person);
			}*/
		}
	}

	private void Player_OnPhobiaMaxed(Phobia phobia)
	{
		if (OnPhobiaMaxed != null)
			OnPhobiaMaxed(phobia);
	}
}