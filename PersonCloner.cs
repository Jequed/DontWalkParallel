using System.Collections.Generic;
using UnityEngine;

public class PersonCloner : DWPObject
{
	[SerializeField]
	private float spawnRate = 1.0f;

	[SerializeField]
	private string pattern = "1";

	[SerializeField]
	private float delay = 0.0f;

    [SerializeField]
    private bool fillPath = false;

    private float elapsedTime = 0.0f;

	private List<NPC> cloneTemplates = new List<NPC>();

	private int patternPosition = 0;

	public float SpawnRate
	{
		get
		{
			return spawnRate;
		}
		set
		{
			spawnRate = value;
		}
	}

	public string Pattern
	{
		get
		{
			return pattern;
		}
		set
		{
			pattern = value;
		}
	}

	public float Delay
	{
		get
		{
			return delay;
		}
		set
		{
			delay = value;
		}
	}

    public bool FillPath
    {
        get
        {
            return fillPath;
        }
        set
        {
            fillPath = value;
        }
    }

    protected override void GameStart()
	{
		base.GameStart();
        
        elapsedTime = spawnRate - delay;

		BoxCollider boxCollider = GetComponent<BoxCollider>();

		foreach (var person in levelContainer.GetComponentsInChildren<NPC>())
		{
			if (person.transform.position.x > transform.position.x + boxCollider.center.x - boxCollider.size.x * 0.5f * transform.localScale.x &&
				person.transform.position.x < transform.position.x + boxCollider.center.x + boxCollider.size.x * 0.5f * transform.localScale.x &&
				person.transform.position.y > transform.position.y + boxCollider.center.y - boxCollider.size.y * 0.5f * transform.localScale.y &&
				person.transform.position.y < transform.position.y + boxCollider.center.y + boxCollider.size.y * 0.5f * transform.localScale.y)
			{
				var clone = Instantiate(person);
				clone.transform.parent = person.transform.parent;
				clone.gameObject.SetActive(false);
				clone.name = "PersonCloneTemplate";
				cloneTemplates.Add(clone);
                Destroy(person.gameObject);
			}
		}

        if (fillPath)
        {
            foreach (var clone in cloneTemplates)
            {
                float maxDuration = clone.ActionController.TotalDuration;

                float currentDuration = delay;
                while (currentDuration < maxDuration)
                {
                    if (pattern[patternPosition] != '0')
                    {
                        // Don't spawn the first one because it will loop back around.
                        if (Mathf.Abs(currentDuration) > 0.01f)
                        {
                            var clones = Spawn();
                            
                            foreach (var clone1 in clones)
                                clone1.ActionController.ElapsedTime = maxDuration - currentDuration;
                        }
                    }

                    patternPosition++;
                    if (patternPosition >= pattern.Length)
                        patternPosition = 0;

                    currentDuration += spawnRate;
                }
                
                elapsedTime = maxDuration - currentDuration + spawnRate;
            }
        }
	}

	protected override void GameUpdate()
	{
		base.GameUpdate();

		elapsedTime += Time.fixedDeltaTime * GlobalData.timeMultiplier;

		if (elapsedTime > spawnRate)
		{
            while (elapsedTime > spawnRate)
                elapsedTime -= spawnRate;

			bool shouldSpawn = (pattern.Length == 0 || pattern[patternPosition] != '0');
			if (shouldSpawn)
				Spawn();

			patternPosition++;
			if (patternPosition >= pattern.Length)
				patternPosition = 0;
		}
	}

	protected virtual List<NPC> Spawn()
	{
        List<NPC> clones = new List<NPC>();
		foreach (var cloneTemplate in cloneTemplates)
		{
			var clone = Instantiate(cloneTemplate);
			clone.transform.parent = cloneTemplate.transform.parent;
			clone.name = "Person";
			clone.gameObject.SetActive(true);
            clone.RandomizeAppearance();
            clone.RandomizeAnimationFrame(10.0f);
            clones.Add(clone);
		}

        return clones;
	}
}