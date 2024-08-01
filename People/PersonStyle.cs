using UnityEngine;

public class PersonStyle : IProbable
{
    public float Probability
    {
        get
        {
            return probability;
        }
    }

    public float probability;
    public Person.Gender gender;
    public Color color = Color.white;

    public PersonStyle(PersonStyleData data)
    {
        probability = data.probability;
        gender = data.gender;
        color = data.color;
    }
}