using UnityEngine;

public class PersonAccessoryInstance : DWPObject
{
    public PersonAccessoryData accessoryData;
    public Color color;
    public bool expanded = true;

    private SpriteRenderer spriteRenderer;
    private PersonAccessory accessory;

    public PersonAccessory Accessory
    {
        get
        {
            return accessory;
        }
    }

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            return spriteRenderer;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        SetAccessory(accessoryData);
        spriteRenderer.color = color;
    }

    public void SetAccessory(PersonAccessoryData data)
    {
        accessoryData = data;
        if (data != null)
            accessory = PersonData.GetAccessoryFromData(data);
    }
}