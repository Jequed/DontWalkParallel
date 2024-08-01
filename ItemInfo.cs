public class ItemInfo
{
	public enum Type
	{
		None,
		Player,
		NPC,
		Block,
		Cloner,
		Destroyer,
        AvoidTile,
        RequiredTile,
        EditorNote
	}

	public Type type;

	public LevelContainer.LayerType Layer
	{
		get
		{
			switch (type)
			{
				case Type.Block:
					return LevelContainer.LayerType.blocks;
				case Type.Player:
				case Type.NPC:
					return LevelContainer.LayerType.people;
				default:
					break;
			}

			return LevelContainer.LayerType.misc;
		}
	}

	public string PrefabName
	{
		get
		{
			return "Prefabs/GamePrefabs/" + type.ToString();
		}
	}

	public string WorkspaceName
	{
		get
		{
			return "Prefabs/EditorPrefabs/" + type.ToString() + "Workspace";
		}
	}

	public ItemInfo(Type type)
	{
		this.type = type;
	}
}