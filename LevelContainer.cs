using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    [SerializeField]
    private DWPCanvas canvasBackground;

    [SerializeField]
    private DWPCanvas canvasShadow;

    [SerializeField]
    private DWPCanvas canvasForeground;

    [SerializeField]
    private GameObject canvasSubTiles;

    [SerializeField]
	private GameObject backgroundsContainer;

	[SerializeField]
	private GameObject blocksContainer;

	[SerializeField]
	private GameObject peopleContainer;

	[SerializeField]
	private GameObject miscContainer;

    [SerializeField]
    private GameObject uiContainer;

	[SerializeField]
	private GameObject phone;

	[SerializeField]
	private SpriteRenderer phoneScreen;

	[SerializeField]
	private new Camera camera;

    [SerializeField]
    private ThoughtBubble thoughtBubble;

    private bool hasBackground = false;

	public enum LayerType
	{
		level,
		blocks,
		people,
		misc,
        art
    }

    public DWPCanvas Canvas_Background
    {
        get
        {
            return canvasBackground;
        }
    }
    public DWPCanvas Canvas_Shadow
    {
        get
        {
            return canvasShadow;
        }
    }
    public DWPCanvas Canvas_Foreground
    {
        get
        {
            return canvasForeground;
        }
    }

    public GameObject CanvasSubTiles
    {
        get
        {
            return canvasSubTiles;
        }
    }

    public bool InitializedCanvases
    {
        get
        {
            return canvasBackground.Inititalized && canvasShadow.Inititalized && canvasForeground.Inititalized;
        }
    }

    public GameObject BackgroundsContainer
    {
        get
        {
            return backgroundsContainer;
        }
    }
    public GameObject BlocksContainer
	{
		get
		{
			return blocksContainer;
		}
	}
	public GameObject PeopleContainer
	{
		get
		{
			return peopleContainer;
		}
	}
	public GameObject MiscContainer
	{
		get
		{
			return miscContainer;
		}
	}

    public GameObject UIContainer
    {
        get
        {
            return uiContainer;
        }
    }

    public GameObject Phone
	{
		get
		{
			return phone;
		}
	}

	public SpriteRenderer PhoneScreen
	{
		get
		{
			return phoneScreen;
		}
	}

	public Camera Camera
	{
		get
		{
			return camera;
		}
	}

	public LevelController Controller
	{
		get
		{
			return GetComponent<LevelController>();
		}
	}

    public bool HasBackgournd
    {
        get
        {
            return hasBackground;
        }
        set
        {
            hasBackground = value;
        }
    }

    public string ThoughtBubbleText
    {
        get
        {
            return thoughtBubble.Text;
        }
        set
        {
            thoughtBubble.Text = value;
        }
    }
}