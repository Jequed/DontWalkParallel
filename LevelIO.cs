using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class LevelIO
{
    private const int VERSION = 5;

    public static void SaveLevel(string path, LevelContainer levelContainer)
    {
        var writer = new BinaryWriter(File.OpenWrite(path));

        writer.Write(VERSION);

        IOUtility.WriteVector3(levelContainer.Phone.transform.localScale, writer);
        IOUtility.WriteVector3(levelContainer.Phone.transform.position, writer);

        writer.Write(levelContainer.ThoughtBubbleText);

        var blocks = levelContainer.BlocksContainer.GetComponentsInChildren<Block>();
        var people = levelContainer.PeopleContainer.GetComponentsInChildren<Person>();
        var miscItems = levelContainer.MiscContainer.GetComponentsInChildren<DWPObject>();

        var allItems = new List<DWPObject>();
        allItems.AddRange(blocks);
        allItems.AddRange(people);
        allItems.AddRange(miscItems);

        writer.Write(allItems.Count);
        foreach (var item in allItems)
        {
            writer.Write((int)item.ItemType);

            IOUtility.WriteVector3(item.transform.position, writer);

            switch (item.ItemType)
            {
                case ItemInfo.Type.Block:
                    SaveBlock(item as Block, writer);
                    break;
                case ItemInfo.Type.NPC:
                case ItemInfo.Type.Player:
                    SavePerson(item as Person, writer);
                    break;
                case ItemInfo.Type.Cloner:
                    SaveCloner(item as PersonCloner, writer);
                    break;
                case ItemInfo.Type.EditorNote:
                    SaveEditorNote(item as EditorNote, writer);
                    break;
                default:
                    break;
            }
        }


        SaveCanvas(levelContainer.Canvas_Background, writer);
        SaveCanvas(levelContainer.Canvas_Shadow, writer);
        SaveCanvas(levelContainer.Canvas_Foreground, writer);


        writer.Close();
    }
    private static void SaveBlock(Block block, BinaryWriter writer)
    {
        writer.Write(block.CanSeeOver);
    }
    private static void SavePerson(Person person, BinaryWriter writer)
    {
        writer.Write((int)person.InitialDirection);

        if (person.ItemType == ItemInfo.Type.NPC)
        {
            var npc = person as NPC;
            writer.Write(npc.Actions.Count);
            foreach (var action in npc.Actions)
            {
                writer.Write(action.expanded);
                writer.Write((int)action.direction);
                writer.Write(action.wait);
                writer.Write(action.waitTime);
                writer.Write(action.distance);
                writer.Write(action.speedMultiplier);
                writer.Write(action.overrideAnimation);
                writer.Write(action.playOnce);
            }
        }

        IOUtility.WriteColor(person.MainSpriteRenderer.color, writer);

        writer.Write(person.AppearanceManager.Accessories.Count);
        foreach (var accessoryInstance in person.AppearanceManager.Accessories)
        {
            writer.Write(accessoryInstance.expanded);
            writer.Write(accessoryInstance.Accessory.name);
            IOUtility.WriteColor(accessoryInstance.color, writer);
        }

        var properties = person.Properties;
        writer.Write(person.Properties.Length);
        foreach (var property in properties)
        {
            var type = property.PropertyType;
            writer.Write((int)type);
            switch (type)
            {
                case PersonProperty.Type.ViewCone:
                    var viewCone = property as ViewCone;
                    writer.Write(viewCone.Radius);
                    writer.Write(viewCone.Arc);
                    writer.Write(viewCone.TurnRate);
                    writer.Write(viewCone.DrainRate);
                    break;
                default:
                    break;
            }
        }
    }
    private static void SaveCloner(PersonCloner cloner, BinaryWriter writer)
    {
        writer.Write(cloner.SpawnRate);
        writer.Write(cloner.Pattern);
        writer.Write(cloner.Delay);
        writer.Write(cloner.FillPath);
    }
    private static void SaveEditorNote(EditorNote note, BinaryWriter writer)
    {
        writer.Write(note.Text);
    }

    private static void SaveCanvas(DWPCanvas canvas, BinaryWriter writer)
    {
        IOUtility.WriteVector3(canvas.transform.position, writer);
        IOUtility.WriteVector3(canvas.Scale, writer);
        writer.Write(canvas.gameObject.activeSelf);

        writer.Write(canvas.Texture.width);
        writer.Write(canvas.Texture.height);
        byte[] bytes = canvas.Texture.EncodeToPNG();
        writer.Write(bytes.Length);
        writer.Write(bytes);
    }


    public static void OpenLevel(string path, LevelContainer levelContainer)
    {
        DeleteChildren(levelContainer.BlocksContainer);
        DeleteChildren(levelContainer.PeopleContainer);
        DeleteChildren(levelContainer.MiscContainer);

        levelContainer.ThoughtBubbleText = "";

        var reader = new BinaryReader(File.OpenRead(path));

        var fileVersion = reader.ReadInt32();

        if (fileVersion >= 2)
        {
            levelContainer.Phone.transform.localScale = IOUtility.ReadVector3(reader);
            levelContainer.Phone.transform.position = IOUtility.ReadVector3(reader);

            if (fileVersion >= 5)
                levelContainer.ThoughtBubbleText = reader.ReadString();
        }

        var itemCount = reader.ReadInt32();

        for (int i = 0; i < itemCount; i++)
        {
            var itemType = (ItemInfo.Type)reader.ReadInt32();

            var itemPosition = IOUtility.ReadVector3(reader);

            DWPObject item = GameObject.Instantiate(Resources.Load<DWPObject>("Prefabs/GamePrefabs/" + itemType.ToString()));

            switch (itemType)
            {
                case ItemInfo.Type.Player:
                    if (fileVersion >= 2)
                        OpenPerson(item as Person, reader, fileVersion);
                    break;
                case ItemInfo.Type.Block:
                    OpenBlock(item as Block, reader, fileVersion);
                    break;
                case ItemInfo.Type.NPC:
                    OpenPerson(item as Person, reader, fileVersion);
                    break;
                case ItemInfo.Type.Cloner:
                    OpenCloner(item as PersonCloner, reader, fileVersion);
                    break;
                case ItemInfo.Type.EditorNote:
                    OpenEditorNote(item as EditorNote, reader, fileVersion);
                    break;
                default:
                    break;
            }
            
            item.transform.position = itemPosition;

            switch (GlobalData.GetInfo(item.ItemType).Layer)
            {
                case LevelContainer.LayerType.blocks:
                    item.transform.parent = levelContainer.BlocksContainer.transform;
                    break;
                case LevelContainer.LayerType.people:
                    item.transform.parent = levelContainer.PeopleContainer.transform;
                    break;
                case LevelContainer.LayerType.misc:
                    item.transform.parent = levelContainer.MiscContainer.transform;
                    break;
            }
        }


        if (fileVersion >= 4)
        {
            OpenCanvas(levelContainer.Canvas_Background, reader);
            OpenCanvas(levelContainer.Canvas_Shadow, reader, true);
            OpenCanvas(levelContainer.Canvas_Foreground, reader);
        }


        reader.Close();
    }
    private static void OpenBlock(Block block, BinaryReader reader, int fileVersion)
    {
        block.CanSeeOver = reader.ReadBoolean();
    }
    private static void OpenPerson(Person person, BinaryReader reader, int fileVersion)
    {
        person.InitialDirection = (ObjectAction.MovementDirection)reader.ReadInt32();

        if (person.ItemType == ItemInfo.Type.NPC)
        {
            List<ObjectAction> actions = new List<ObjectAction>();

            int actionCount = reader.ReadInt32();
            for (int i = 0; i < actionCount; i++)
            {
                ObjectAction action = new ObjectAction();
                action.expanded = reader.ReadBoolean();
                action.direction = (ObjectAction.MovementDirection)reader.ReadInt32();
                action.wait = reader.ReadBoolean();
                action.waitTime = reader.ReadSingle();
                action.distance = reader.ReadSingle();
                action.speedMultiplier = reader.ReadSingle();
                action.overrideAnimation = reader.ReadBoolean();
                action.playOnce = reader.ReadBoolean();
                actions.Add(action);
            }
            (person as NPC).Actions = actions;
        }

        if (fileVersion >= 1)
        {
            person.MainSpriteRenderer.color = IOUtility.ReadColor(reader);

            int accessoryCount = reader.ReadInt32();
            for (int i = 0; i < accessoryCount; i++)
            {
                bool expanded = reader.ReadBoolean();
                string accessoryName = reader.ReadString();
                Color accessoryColor = IOUtility.ReadColor(reader);

                PersonAccessory foundAccessory = null;
                foreach (var accessory in PersonData.accessories)
                {
                    if (accessory.name == accessoryName)
                    {
                        foundAccessory = accessory;
                        break;
                    }
                }

                if (foundAccessory != null)
                {
                    var instance = person.AppearanceManager.AddAccessory(foundAccessory);
                    instance.expanded = expanded;
                    instance.color = accessoryColor;
                }
            }
        }

        if (fileVersion >= 2)
        {
            int propertyCount = reader.ReadInt32();

            for (int i = 0; i < propertyCount; i++)
            {
                var propertyType = (PersonProperty.Type)reader.ReadInt32();

                var property = person.AddProperty(propertyType);

                switch (propertyType)
                {
                    case PersonProperty.Type.ViewCone:
                        var viewCone = property as ViewCone;
                        viewCone.Radius = reader.ReadSingle();
                        viewCone.Arc = reader.ReadSingle();
                        viewCone.TurnRate = reader.ReadSingle();
                        if (fileVersion >= 3)
                            viewCone.DrainRate = reader.ReadSingle();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    private static void OpenCloner(PersonCloner cloner, BinaryReader reader, int fileVersion)
    {
        cloner.SpawnRate = reader.ReadSingle();
        cloner.Pattern = reader.ReadString();
        cloner.Delay = reader.ReadSingle();
        if (fileVersion >= 2)
            cloner.FillPath = reader.ReadBoolean();
    }
    private static void OpenEditorNote(EditorNote note, BinaryReader reader, int fileVersion)
    {
        note.Text = reader.ReadString();
    }

    private static void OpenCanvas(DWPCanvas canvas, BinaryReader reader, bool isShadow = false)
    {
        canvas.transform.position = IOUtility.ReadVector3(reader);
        var scale = IOUtility.ReadVector3(reader);
        canvas.gameObject.gameObject.SetActive(reader.ReadBoolean());

        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int byteCount = reader.ReadInt32();
        byte[] bytes = reader.ReadBytes(byteCount);

        if (width > 0 && height > 0)
        {
            canvas.Initialize(scale.x, scale.y, width, height);
            canvas.Texture.LoadImage(bytes);
        }
    }

    private static void DeleteChildren(GameObject container)
    {
        var items = container.GetComponentsInChildren<DWPObject>();
        foreach (var item in items)
            GameObject.Destroy(item.gameObject);
    }
}