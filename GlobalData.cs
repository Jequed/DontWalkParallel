using System;
using System.Collections.Generic;

public static class GlobalData
{
	public static Player player;

    public static NPC[] allNPCs;

	public static bool playMode = false;

	public static bool debugMode = false;

    public static bool displayBlocksInPlayMode = true;

	public static float timeMultiplier = 2.0f;
    public const float phobiaTimeMultiplier = 0.5f;
    public const float defaultTimeMultiplier = 2.0f;

    public static System.Random random;

    public static string error = "";

    public const float phoneAspectRatio = 16.0f / 9.0f;

	public const int defaultCameraMask = -289;
	public const int phoneCameraMask = 100000000;

    public const int spritePixelSize = 96;

    private static ItemInfo[] itemInfos;

	private static Dictionary<ItemInfo.Type, ItemInfo> itemInfoDictionary = new Dictionary<ItemInfo.Type, ItemInfo>();

	static GlobalData()
	{
        random = new System.Random(12345);

        itemInfos = new ItemInfo[Enum.GetValues(typeof(ItemInfo.Type)).Length];
        int currentItemInfo = 0;
        foreach (ItemInfo.Type type in Enum.GetValues(typeof(ItemInfo.Type)))
            itemInfos[currentItemInfo++] = new ItemInfo(type);

        foreach (var info in itemInfos)
            itemInfoDictionary.Add(info.type, info);

        PersonData.Initialize();
	}

	public static ItemInfo GetInfo(ItemInfo.Type type)
	{
		if (!itemInfoDictionary.ContainsKey(type))
			return null;

		return itemInfoDictionary[type];
	}
}