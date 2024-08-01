using UnityEngine;

public class PersonDestroyer : DWPObject
{
	private BoxCollider boxCollider;

	protected override void GameStart()
	{
		base.GameStart();

		boxCollider = GetComponent<BoxCollider>();
	}

	protected override void GameUpdate()
	{
		base.GameUpdate();

		foreach (var person in GlobalData.allNPCs)
		{
			if (person.transform.position.x > transform.position.x + boxCollider.center.x - boxCollider.size.x * 0.5f * transform.localScale.x &&
				person.transform.position.x < transform.position.x + boxCollider.center.x + boxCollider.size.x * 0.5f * transform.localScale.x &&
				person.transform.position.y > transform.position.y + boxCollider.center.y - boxCollider.size.y * 0.5f * transform.localScale.y &&
				person.transform.position.y < transform.position.y + boxCollider.center.y + boxCollider.size.y * 0.5f * transform.localScale.y)
			{
				Destroy(person.gameObject);
			}
		}
	}
}