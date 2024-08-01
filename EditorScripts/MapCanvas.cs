using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    private Mesh mesh;

    private Vector3 scale;

    private Texture2D texture;

    Vector3 bottomLeft;
    Vector3 topRight;

    private CanvasLineDrawer canvasLineDrawer;

    private const float downsize = 10.0f;
    private const float width = 1920.0f / downsize;
    private const float height = 1080.0f / downsize;

    private const float depth = 10.0f;

    private const int texResolution = 2;
    private const int texWidth = 1920 * texResolution;
    private const int texHeight = 1080 * texResolution;

    private readonly Color clearColor = new Color32(138, 165, 112, 255);

    public Texture2D Texture
    {
        get
        {
            if (texture == null)
            {
                texture = new Texture2D(texWidth, texHeight);
                Color32[] pixels = new Color32[texWidth * texHeight];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = clearColor;
                texture.SetPixels32(pixels);
                texture.Apply();
            }

            return texture;
        }
    }

    void Start()
    {
        scale = new Vector3(width, height, 1.0f);
        bottomLeft = new Vector3(-scale.x * 0.5f, -scale.y * 0.5f);
        topRight = new Vector3(scale.x * 0.5f, scale.y * 0.5f);

        canvasLineDrawer = new CanvasLineDrawer();

        mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = new Vector3[]
        {
           new Vector3(-scale.x * 0.5f, -scale.y * 0.5f, depth),
           new Vector3(-scale.x * 0.5f, scale.y * 0.5f, depth),
           new Vector3(scale.x * 0.5f, scale.y * 0.5f, depth),
           new Vector3(scale.x * 0.5f, -scale.y * 0.5f, depth)
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

        var material = GetComponent<MeshRenderer>().material;
        material.mainTexture = Texture;
    }

    public void DrawLine(Vector3 position1, Vector3 position2)
    {
        List<int> points = new List<int>();

        int startX, startY;
        GetTextureCoordinates(position1, out startX, out startY);
        int endX, endY;
        GetTextureCoordinates(position2, out endX, out endY);

        DrawLine(startX, startY, endX, endY, points);

        DrawPoints(points, Color.black);
    }

    public void Erase(Vector3 position1, Vector3 position2, int size)
    {
        List<int> points = new List<int>();

        int startX, startY;
        GetTextureCoordinates(position1, out startX, out startY);
        int endX, endY;
        GetTextureCoordinates(position2, out endX, out endY);

        for (int x = -size / 2; x < size / 2; x++)
        {
            for (int y = -size / 2; y < size / 2; y++)
            {
                DrawLine(startX + x, startY + y, endX + x, endY + y, points);
            }
        }

        DrawPoints(points, clearColor);
    }

    private void DrawLine(int startX, int startY, int endX, int endY, List<int> points)
    {
        canvasLineDrawer.Reset(startX, startY, endX, endY);

        int x = startX, y = startY;
        do
        {
            points.Add(GetIndex(x, y));
        }
        while (canvasLineDrawer.GetNext(ref x, ref y));
    }

    private void DrawPoints(List<int> points, Color color)
    {
        var pixels = Texture.GetPixels32();

        foreach (var point in points)
        {
            if (point >= 0 && point < texWidth * texHeight)
                pixels[point] = color;
        }

        Texture.SetPixels32(pixels);
        Texture.Apply();
    }

    private void GetTextureCoordinates(Vector3 worldPosition, out int x, out int y)
    {
        float fx = (worldPosition.x - bottomLeft.x) / (topRight.x - bottomLeft.x);
        float fy = (worldPosition.y - bottomLeft.y) / (topRight.y - bottomLeft.y);

        x = (int)Mathf.Floor(fx * (float)texWidth);
        y = (int)Mathf.Floor(fy * (float)texHeight);
    }

    private int GetIndex(int x, int y)
    {
        return x + y * texWidth;
    }

    private class CanvasLineDrawer
    {
        private int iDrawLineStartX, iDrawLineStartY;
        private int iDrawLineEndX, iDrawLineEndY;
        private int iDrawLineDX, iDrawLineDY;
        private int iDrawLineSX, iDrawLineSY;
        private int iDrawLineErr;
        private int iDrawLineX, iDrawLineY;

        public void Reset(int iStartX, int iStartY, int iEndX, int iEndY)
        {
            iDrawLineStartX = iStartX; iDrawLineStartY = iStartY;
            iDrawLineEndX = iEndX; iDrawLineEndY = iEndY;
            iDrawLineDX = Mathf.Abs(iDrawLineEndX - iDrawLineStartX);
            iDrawLineDY = Mathf.Abs(iDrawLineEndY - iDrawLineStartY);
            if (iDrawLineStartX < iDrawLineEndX)
            {
                iDrawLineSX = 1;
            }
            else
            {
                iDrawLineSX = -1;
            }
            if (iDrawLineStartY < iDrawLineEndY)
            {
                iDrawLineSY = 1;
            }
            else
            {
                iDrawLineSY = -1;
            }
            iDrawLineErr = iDrawLineDX - iDrawLineDY;
            iDrawLineX = iDrawLineStartX; iDrawLineY = iDrawLineStartY;
        }
        public bool GetNext(ref int iX, ref int iY)
        {
            iX = iDrawLineX; iY = iDrawLineY;
            if (iDrawLineX == iDrawLineEndX && iDrawLineY == iDrawLineEndY)
            {
                return false;
            }
            int e2 = 2 * iDrawLineErr;
            if (e2 > -iDrawLineDY)
            {
                iDrawLineErr -= iDrawLineDY;
                iDrawLineX += iDrawLineSX;
            }
            if (e2 < iDrawLineDX)
            {
                iDrawLineErr += iDrawLineDX;
                iDrawLineY += iDrawLineSY;
            }
            return true;
        }
    }
}