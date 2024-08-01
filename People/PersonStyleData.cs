using UnityEngine;

[CreateAssetMenu(fileName = "Style", menuName = "DontWalkParallel/Style", order = 3)]
public class PersonStyleData : ScriptableObject
{
    public float probability;
    public Person.Gender gender;
    public Color color = Color.white;
}