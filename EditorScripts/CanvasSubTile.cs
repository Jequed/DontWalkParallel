using UnityEngine;

public class CanvasSubTile : MonoBehaviour
{
    private Texture2D texture;

    private Color[] newPixels;

    private bool isDirty = true;

    private void Awake()
    {
        texture = new Texture2D(GlobalData.spritePixelSize, GlobalData.spritePixelSize);
        texture.filterMode = FilterMode.Point;

        newPixels = new Color[GlobalData.spritePixelSize * GlobalData.spritePixelSize];

        Vector3[] vertices = new Vector3[]
        {
           new Vector3(-0.5f, -0.5f, -3.0f),
           new Vector3(-0.5f, 0.5f, -3.0f),
           new Vector3(0.5f, 0.5f, -3.0f),
           new Vector3(0.5f, -0.5f, -3.0f)
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

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        GetComponent<MeshRenderer>().material.mainTexture = texture;

        Clear();
    }

    public void Apply()
    {
        if (isDirty)
        {
            texture.SetPixels(newPixels);
            texture.Apply();
        }
    }
    public void Clear()
    {
        if (isDirty)
        {
            for (int i = 0; i < newPixels.Length; i++)
                newPixels[i] = Color.clear;

            Apply();

            isDirty = false;
        }
    }
    public void SetPixel(int x, int y, Color color)
    {
        isDirty = true;

        int index = x + y * GlobalData.spritePixelSize;

        if (index >= 0 && index < newPixels.Length)
            newPixels[x + y * GlobalData.spritePixelSize] = color;
    }
}