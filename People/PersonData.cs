using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class PersonData
{
    public static List<PersonAnimation> personAnimations;
    public static List<PersonStyle> personStyles;
    public static List<PersonAccessory> accessories;

    private static Dictionary<PersonAccessoryData, PersonAccessory> accessoryDataMap = new Dictionary<PersonAccessoryData, PersonAccessory>();

    private static readonly string imagesDirectory = Application.dataPath + "/Resources/Images/";
    private static readonly string resourceDirectoryFile = Application.dataPath + "/Resources/PersonData.txt";

    const string assetExtension = ".asset";
    const string pngExtension = ".png";

    public static void Initialize()
    {
        #if UNITY_EDITOR
            BuildResourceDirectoryFile();
        #endif

        //TempRead();

        var stringList = new List<string>(Resources.Load<TextAsset>("PersonData").text.Split('\r', '\n'));
        stringList.Reverse();
        Stack<string> reader = new Stack<string>();
        foreach (var str in stringList)
        {
            if (str.Length > 0)
                reader.Push(str);
        }

        personAnimations = new List<PersonAnimation>();
        personStyles = new List<PersonStyle>();
        accessories = new List<PersonAccessory>();

        LoadAllAnimationsAndStylesInDirectory(personAnimations, personStyles, reader);

        LoadAccessories(reader);
    }

    public static PersonAccessory GetAccessoryFromData(PersonAccessoryData data)
    {
        return accessoryDataMap[data];
    }

    private static void LoadAccessories(Stack<string> reader)
    {
        var accessoryCount = int.Parse(reader.Pop());

        for (int i = 0; i < accessoryCount; i++)
        {
            var fileString = reader.Pop();

            var tempString = fileString.Substring(0, fileString.LastIndexOf("\\"));

            var accessoryData = Resources.Load<PersonAccessoryData>(fileString);
            var accessory = new PersonAccessory(accessoryData, tempString.Substring(tempString.LastIndexOf("\\") + 1));

            if (accessory != null)
            {
                LoadAllAnimationsAndStylesInDirectory(accessory.animations, accessory.styles, reader);
                accessories.Add(accessory);
                accessoryDataMap.Add(accessoryData, accessory);
            }
        }
    }

    private static void LoadAllAnimationsAndStylesInDirectory(List<PersonAnimation> animations, List<PersonStyle> styles, Stack<string> reader)
    {
        LoadAllAnimationsInDirectory(animations, reader);
        LoadStyles(styles, reader);
    }

    private static void LoadAllAnimationsInDirectory(List<PersonAnimation> animations, Stack<string> reader)
    {
        int animationCount = int.Parse(reader.Pop());
        for (int i = 0; i < animationCount; i++)
            LoadAnimations(animations, reader);
    }

    private static void LoadAnimations(List<PersonAnimation> animations, Stack<string> reader)
    {
        string movementTypeString = reader.Pop();

        var popper = reader.Pop();
        var imageCount = int.Parse(popper);
        
        Sprite[] sprites = new Sprite[imageCount];
        for (int i = 0; i < imageCount; i++)
        {
            var sprite = Resources.Load<Sprite>(reader.Pop());
            sprites[int.Parse(sprite.name)] = sprite;
        }

        var imageSeriesData = Resources.Load<ImageSeriesData>(reader.Pop());
        var imageSeries = new ImageSeries(imageSeriesData, sprites);

        const string sideways = "Sideways";
        Person.MovementType movementType = Person.MovementType.none;
        if (movementTypeString.EndsWith(sideways))
        {
            imageSeries.flipX = true;
            var imageSeries2 = new ImageSeries(imageSeriesData, sprites);
            imageSeries2.flipX = false;

            animations.Add(new PersonAnimation(imageSeries, Person.BehaviorType.neutral, (Person.MovementType)Enum.Parse(typeof(Person.MovementType), movementTypeString.Replace(sideways, "Left"))));
            animations.Add(new PersonAnimation(imageSeries2, Person.BehaviorType.neutral, (Person.MovementType)Enum.Parse(typeof(Person.MovementType), movementTypeString.Replace(sideways, "Right"))));
        }
        else
        {
            movementType = (Person.MovementType)Enum.Parse(typeof(Person.MovementType), movementTypeString);

            animations.Add(new PersonAnimation(imageSeries, Person.BehaviorType.neutral, movementType));
        }
    }

    private static void LoadStyles(List<PersonStyle> styles, Stack<string> reader)
    {
        var styleCount = int.Parse(reader.Pop());
        for (int i = 0; i < styleCount; i++)
        {
            var data = Resources.Load<PersonStyleData>(reader.Pop());
            var style = new PersonStyle(data);
            styles.Add(style);
        }
    }


    // Resource Directory File

    public static void BuildResourceDirectoryFile()
    {
        File.WriteAllText(resourceDirectoryFile, string.Empty);
        StreamWriter writer = new StreamWriter(File.OpenWrite(resourceDirectoryFile));

        FindAllAnimationsAndStylesInDirectory(new DirectoryInfo(imagesDirectory + "People"), writer);

        FindAccessories(new DirectoryInfo(imagesDirectory + "Accessories"), writer);

        writer.Close();
    }

    private static void FindAccessories(DirectoryInfo directoryInfo, StreamWriter writer)
    {
        var directories = directoryInfo.GetDirectories();
        writer.WriteLine(directories.Length);
        foreach (var info in directories)
        {
            string infoString = info.ToString();

            FileInfo[] dataFiles = info.GetFiles("*" + assetExtension);
            foreach (var file in dataFiles)
            {
                string fileString = file.ToString();
                WriteResource(fileString.Substring(0, fileString.Length - assetExtension.Length), writer);
                break;
            }

            FindAllAnimationsAndStylesInDirectory(info, writer);
        }
    }

    private static void FindAllAnimationsAndStylesInDirectory(DirectoryInfo directoryInfo, StreamWriter writer)
    {
        FindAllAnimationsInDirectory(new DirectoryInfo(directoryInfo.ToString() + "/Animations/"), writer);
        FindStyles(new DirectoryInfo(directoryInfo.ToString() + "/Styles/"), writer);
    }

    private static void FindAllAnimationsInDirectory(DirectoryInfo directoryInfo, StreamWriter writer)
    {
        var directories = directoryInfo.GetDirectories();
        writer.WriteLine(directories.Length);
        foreach (var info in directories)
            FindAnimations(info, writer);
    }

    private static void FindAnimations(DirectoryInfo directoryInfo, StreamWriter writer)
    {
        string infoString = directoryInfo.ToString();
        string movementTypeString = infoString.Substring(infoString.LastIndexOf("\\") + 1);
        writer.WriteLine(movementTypeString);

        FileInfo[] imageFiles = directoryInfo.GetFiles("*" + pngExtension);
        writer.WriteLine(imageFiles.Length);
        foreach (var file in imageFiles)
        {
            string fileString = file.ToString();
            WriteResource(fileString.Substring(0, fileString.Length - pngExtension.Length), writer);
        }

        FileInfo[] dataFiles = directoryInfo.GetFiles("*" + assetExtension);
        foreach (var file in dataFiles)
        {
            string fileString = file.ToString();
            WriteResource(fileString.Substring(0, fileString.Length - assetExtension.Length), writer);
            break;
        }
    }

    private static void FindStyles(DirectoryInfo directoryInfo, StreamWriter writer)
    {
        FileInfo[] files = directoryInfo.GetFiles("*" + assetExtension);
        writer.WriteLine(files.Length);
        foreach (var file in files)
        {
            string fileString = file.ToString();
            WriteResource(fileString.Substring(0, fileString.Length - assetExtension.Length), writer);
        }
    }

    private static void WriteResource(string path, StreamWriter writer)
    {
        const string resources = "\\Resources\\";
        string resourcePath = path.Substring(path.LastIndexOf(resources) + resources.Length);
        writer.WriteLine(resourcePath);
    }
}