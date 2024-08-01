using UnityEngine;

public abstract class PersonProperty : DWPObject
{
    public enum Type
    {
        ViewCone,
        Approacher,
        DontLookAt
    }

    public bool expanded = true;

    protected Person person;

    public abstract Type PropertyType { get; }

    protected override void Start()
    {
        base.Start();

        person = GetComponentInParent<Person>();
    }
}