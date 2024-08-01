using System.Collections.Generic;

[System.Serializable]
public class PersonAccessory : IProbable
{
    public float Probability
    {
        get
        {
            return probability;
        }
    }

    public enum AccessoryType
    {
        shirt = 0,
        pants = 1,
        hair = 2,
        shoes = 3,
        backpack = 4
    }

    public PersonAccessoryData Data
    {
        get
        {
            return data;
        }
    }

    public float probability;
    public string name;
    public AccessoryType type;
    public Person.Gender gender;
    public List<PersonAnimation> animations = new List<PersonAnimation>();
    public List<PersonStyle> styles = new List<PersonStyle>();

    private PersonAccessoryData data;

    public PersonAccessory(PersonAccessoryData data, string name)
    {
        this.data = data;

        probability = data.probability;
        type = data.type;
        gender = data.gender;

        this.name = name;
    }
}