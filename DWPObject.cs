using UnityEngine;

public abstract class DWPObject : MonoBehaviour
{
	[SerializeField]
	private ItemInfo.Type itemType;

	protected LevelContainer levelContainer
	{
		get
		{
			return GetComponentInParent<LevelContainer>();
		}
	}

	public ItemInfo Info
	{
		get
		{
			return GlobalData.GetInfo(itemType);
		}
	}

	public ItemInfo.Type ItemType
	{
		get
		{
			return itemType;
		}
	}

	public LevelContainer.LayerType Layer
	{
		get
		{
			return Info.Layer;
		}
	}

	protected bool PlayMode
	{
		get
		{
			return GlobalData.playMode;
		}
	}

	protected virtual void Awake()
	{
		if (GlobalData.playMode)
			GameAwake();
		else
			EditorAwake();
	}
	protected virtual void EditorAwake() { }
	protected virtual void GameAwake() { }

	protected virtual void Start()
	{
		if (GlobalData.playMode)
			GameStart();
		else
			EditorStart();
	}
	protected virtual void EditorStart() { }
	protected virtual void GameStart() { }

	protected virtual void FixedUpdate()
	{
		if (GlobalData.playMode)
			GameUpdate();
		else
			EditorUpdate();
	}
	protected virtual void EditorUpdate() { }
	protected virtual void GameUpdate() { }

    protected virtual void OnDestroy() { }
}