using System;
using System.Collections.Generic;
using UnityEngine;

public class PersonAppearanceManager
{
    /*private class AccessoryInstance
    {
        public PersonAccessory accessory;
        public PersonStyle style;
        public SpriteRenderer spriteRenderer;

        public AccessoryInstance(PersonAccessory accessory, PersonStyle style, SpriteRenderer spriteRenderer)
        {
            this.accessory = accessory;
            this.style = style;
            this.spriteRenderer = spriteRenderer;
        }
    }*/

    private Person person;

    private PersonStyle style;
    private List<PersonAccessoryInstance> accessories = new List<PersonAccessoryInstance>();

    public List<PersonAccessoryInstance> Accessories
    {
        get
        {
            return accessories;
        }
    }

    public PersonAppearanceManager(Person person)
    {
        this.person = person;

        foreach (var instance in person.MainSpriteRenderer.GetComponentsInChildren<PersonAccessoryInstance>())
            accessories.Add(instance);
    }

    public void UpdateImages(Person.BehaviorType behaviorType, Person.MovementType movementType)
    {
        foreach (var instance in accessories)
        {
            var accessory = PersonData.GetAccessoryFromData(instance.accessoryData);
            bool foundAnimation = false;
            foreach (var animation in accessory.animations)
            {
                if (animation.behavior == behaviorType && animation.movement == movementType)
                {
                    foundAnimation = true;
                    person.UpdateSprite(instance.SpriteRenderer, animation.imageSeries);
                    break;
                }
            }
            if (!foundAnimation)
                instance.SpriteRenderer.sprite = null;
        }
    }

    public void Randomize()
    {
        ClearAccessories();

        //person.gender = (UnityEngine.Random.value > 0.5f) ? Person.Gender.male : Person.Gender.female;
        //if (UnityEngine.Random.value < 0.001f)
        //    person.gender = Person.Gender.neutral;

        person.gender = (GlobalData.random.Next(2) == 1) ? Person.Gender.male : Person.Gender.female;
        if (GlobalData.random.Next(1000) == 0)
            person.gender = Person.Gender.neutral;

        style = PersonData.personStyles[Choose(PersonData.personStyles.ToArray())];
        person.MainSpriteRenderer.color = style.color;

        var accessoryList = PersonData.accessories;
        foreach (var accessoryType in Enum.GetValues(typeof(PersonAccessory.AccessoryType)))
        {
            var validAccessories = new List<PersonAccessory>();
            foreach (var accessory in accessoryList)
            {
                if (accessory.type == (PersonAccessory.AccessoryType)accessoryType)
                {
                    if (person.gender == accessory.gender || accessory.gender == Person.Gender.neutral || person.gender == Person.Gender.neutral)
                        validAccessories.Add(accessory);
                }
            }

            if (validAccessories.Count > 0)
                AddAccessory(validAccessories[Choose(validAccessories.ToArray())]);
        }
    }

    public PersonAccessoryInstance AddAccessory(PersonAccessory accessory)
    {
        var accessoryStyle = accessory.styles[Choose(accessory.styles.ToArray())];

        var newGameObject = new GameObject(accessory.type.ToString());
        newGameObject.transform.parent = person.MainSpriteRenderer.transform;

        var spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();
        var parentChildrenCount = person.MainSpriteRenderer.GetComponentsInChildren<PersonAccessoryInstance>().Length;
        spriteRenderer.transform.localPosition = Vector3.zero;
        spriteRenderer.transform.localScale = Vector3.one;

        var accessoryInstance = newGameObject.AddComponent<PersonAccessoryInstance>();
        accessoryInstance.accessoryData = accessory.Data;
        accessoryInstance.color = accessoryStyle.color;

        accessories.Add(accessoryInstance);

        RefreshZOrders();

        return accessoryInstance;
    }
    public void RemoveAccessory(PersonAccessoryInstance accessoryInstance)
    {
        GameObject.Destroy(accessoryInstance.gameObject);
        accessories.Remove(accessoryInstance);

        RefreshZOrders();
    }
    public void ClearAccessories()
    {
        foreach (var accessoryInstance in accessories)
            GameObject.Destroy(accessoryInstance.gameObject);

        accessories.Clear();
    }

    public void RefreshZOrders()
    {
        float zOrder = -0.01f;
        foreach (var accessory in accessories)
        {
            accessory.transform.localPosition = new Vector3(accessory.transform.localPosition.x, accessory.transform.localPosition.y, zOrder);
            zOrder -= 0.01f;
        }
    }

    private int Choose(IProbable[] probables)
    {
        float probabilityTotal = 0.0f;
        foreach (var probable in probables)
            probabilityTotal += probable.Probability;

        //float random = UnityEngine.Random.value * probabilityTotal;

        float random = (GlobalData.random.Next(10000) / 10000.0f) * probabilityTotal;

        float currentValue = 0.0f;
        for (int i = 0; i < probables.Length; i++)
        {
            currentValue += probables[i].Probability;

            if (random <= currentValue)
                return i;
        }

        return probables.Length - 1;
    }
}