using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Accessory", menuName = "DontWalkParallel/Accessory", order = 2)]
public class PersonAccessoryData : ScriptableObject
{
    public float probability;
    public PersonAccessory.AccessoryType type;
    public Person.Gender gender;
}