using UnityEngine;

public class MapLevel : DWPObject
{
    [SerializeField]
    private MeshFilter mainMeshFilter;

    [SerializeField]
    private MeshFilter borderMeshFilter;

    [SerializeField]
    private TextMesh levelNameText;

    private string levelName = "";

    private int difficulty = 0;

    private Vector3 position;

    private Vector3 scale;
    private Vector3 borderScale;

    private const float downsize = 500.0f;
    private const float width = 1920.0f / downsize;
    private const float height = 1080.0f / downsize;

    private const float mainDepth = -0.1f;
    private const float borderDepth = 0.0f;

    private static readonly Color[] difficultyColors = new Color[]
    {
        Color.white,
        Color.cyan,
        Color.green,
        Color.yellow,
        new Color(1.0f, 0.5f, 0.0f),
        Color.red,
        new Color(0.5f, 0.0f, 1.0f),
        Color.magenta,
        new Color(0.5f, 0.0f, 0.25f)
    };

    public string LevelName
    {
        get
        {
            return levelNameText.text;
        }
        set
        {
            levelNameText.text = value;

            var texture = Resources.Load<Texture>("Images/LevelPreviews/" + value);

            if (texture != null)
                mainMeshFilter.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }

    public int Difficulty
    {
        get
        {
            return difficulty;
        }
        set
        {
            difficulty = value;

            borderMeshFilter.GetComponent<MeshRenderer>().material.color = difficultyColors[value];
        }
    }

    public Vector3 Scale
    {
        get
        {
            return scale;
        }
    }

    public Vector3 BorderScale
    {
        get
        {
            return borderScale;
        }
    }

    public static int MinDifficulty
    {
        get
        {
            return 0;
        }
    }
    public static int MaxDifficulty
    {
        get
        {
            return difficultyColors.Length - 1;
        }
    }

    protected override void Start()
    {
        base.Start();

        scale = new Vector3(width, height, 1.0f);
        borderScale = new Vector3(width * 1.2f, height * 1.3f);

        GetComponent<BoxCollider>().size = new Vector3(width, height, 1.0f);

        CreateMainMesh();

        CreateBorderMesh();

        Difficulty = difficulty;
    }

    private void CreateMainMesh()
    {
        var mesh = mainMeshFilter.mesh;

        Vector3[] vertices = new Vector3[]
        {
           new Vector3(-scale.x * 0.5f, -scale.y * 0.5f, mainDepth),
           new Vector3(-scale.x * 0.5f, scale.y * 0.5f, mainDepth),
           new Vector3(scale.x * 0.5f, scale.y * 0.5f, mainDepth),
           new Vector3(scale.x * 0.5f, -scale.y * 0.5f, mainDepth)
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        Vector2[] uvs = new Vector2[]
        {
           new Vector2(0.0f, 0.0f),
           new Vector2(0.0f, 1.0f),
           new Vector2(1.0f, 1.0f),
           new Vector2(1.0f, 0.0f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    private void CreateBorderMesh()
    {
        var mesh = borderMeshFilter.mesh;

        Vector3[] vertices = new Vector3[]
        {
           new Vector3(-borderScale.x * 0.5f, -borderScale.y * 0.5f, borderDepth),
           new Vector3(-borderScale.x * 0.5f, borderScale.y * 0.5f, borderDepth),
           new Vector3(borderScale.x * 0.5f, borderScale.y * 0.5f, borderDepth),
           new Vector3(borderScale.x * 0.5f, -borderScale.y * 0.5f, borderDepth)
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}