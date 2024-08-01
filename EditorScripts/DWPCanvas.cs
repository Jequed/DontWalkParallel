using System;
using System.Collections.Generic;
using UnityEngine;

public class DWPCanvas : MonoBehaviour
{
    private Vector3 scale;

    private Texture2D texture;

    private int texWidth;
    private int texHeight;

    private Vector3 bottomLeft;
    private Vector3 topRight;

    private CanvasLineDrawer canvasLineDrawer;

    private bool initialized = false;

    private Color[] newPixels;
    private CanvasSubTile[] subTiles;

    public bool Inititalized
    {
        get
        {
            return initialized;
        }
    }

    public Vector3 Scale
    {
        get
        {
            return scale;
        }
    }

    public Texture2D Texture
    {
        get
        {
            if (texture == null)
            {
                texture = new Texture2D(texWidth, texHeight);
                //texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
            }

            return texture;
        }
    }

    public void Initialize(float width, float height, int texWidth, int texHeight)
    {
        this.texWidth = texWidth;
        this.texHeight = texHeight;

        scale = new Vector3(width, height, 1.0f);
        bottomLeft = -new Vector3(scale.x * 0.5f, scale.y * 0.5f);
        topRight = new Vector3(scale.x * 0.5f, scale.y * 0.5f);

        canvasLineDrawer = new CanvasLineDrawer();

        Vector3[] vertices = new Vector3[]
        {
           new Vector3(-scale.x * 0.5f, -scale.y * 0.5f, 0.0f),
           new Vector3(-scale.x * 0.5f, scale.y * 0.5f, 0.0f),
           new Vector3(scale.x * 0.5f, scale.y * 0.5f, 0.0f),
           new Vector3(scale.x * 0.5f, -scale.y * 0.5f, 0.0f)
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

        GetComponent<MeshRenderer>().material.mainTexture = Texture;

        initialized = true;
    }

    public void Clear(Color clearColor)
    {
        Color32[] pixels = new Color32[texWidth * texHeight];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clearColor;
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    public void BeginDrawing(CanvasSubTile[] subTiles)
    {
        this.subTiles = subTiles;

        newPixels = texture.GetPixels();
    }
    public void EndDrawing()
    {
        ClearSubTiles();

        Texture.SetPixels(newPixels);
        Texture.Apply();

        newPixels = null;
    }
    public void EndDrawing(Vector3[] points, Color color, int size)
    {
        ClearSubTiles();

        BeginDrawing(null);

        DrawLines(points, color, size);

        newPixels = null;
    }

    public void DrawLines(Vector3[] positions, Color color, int size)
    {
        List<Int2> points = new List<Int2>();

        if (size != 1)
        {
            for (int i = 1; i < positions.Length; i++)
            {
                Int2 start = GetTextureCoordinates(positions[i - 1]);
                Int2 end = GetTextureCoordinates(positions[i]);

                for (int x = -size / 2; x < size / 2; x++)
                {
                    for (int y = -size / 2; y < size / 2; y++)
                    {
                        DrawLineInternal(new Int2(start.x + x, start.y + y), new Int2(end.x + x, end.y + y), points);
                    }
                }
            }
        }
        else
        {
            for (int i = 1; i < positions.Length; i++)
            {
                Int2 start = GetTextureCoordinates(positions[i - 1]);
                Int2 end = GetTextureCoordinates(positions[i]);

                DrawLineInternal(start, end, points);
            }
        }

        DrawPoints(points, color);
    }
    public void DrawLine(Vector3 position1, Vector3 position2, Color color, int size)
    {
        List<Int2> points = new List<Int2>();
        
        Int2 start = GetTextureCoordinates(position1);
        Int2 end = GetTextureCoordinates(position2);

        if (size != 1)
        {
            for (int x = -size / 2; x < size / 2; x++)
            {
                for (int y = -size / 2; y < size / 2; y++)
                {
                    DrawLineInternal(new Int2(start.x + x, start.y + y), new Int2(end.x + x, end.y + y), points);
                }
            }
        }
        else
        {
            DrawLineInternal(start, end, points);
        }

        DrawPoints(points, color);
    }

    public void Fill(Vector3 position, Color color)
    {
        Int2 start = GetTextureCoordinates(position);
        int startIndex = GetIndex(start);
        
        if (startIndex > 0)
        {
            var startColor = newPixels[startIndex];

            if (startColor != color)
            {
                LinkedList<Int2> pixelsToProcess = new LinkedList<Int2>();

                pixelsToProcess.AddFirst(start);

                Int2[] tempPositions = new Int2[4];
                while (pixelsToProcess.Count > 0)
                {
                    Int2 firstPosition = pixelsToProcess.First.Value;

                    tempPositions[0] = new Int2(firstPosition.x - 1, firstPosition.y); //left
                    tempPositions[1] = new Int2(firstPosition.x + 1, firstPosition.y); //right
                    tempPositions[2] = new Int2(firstPosition.x, firstPosition.y + 1); //up
                    tempPositions[3] = new Int2(firstPosition.x, firstPosition.y - 1); //down

                    foreach (var tempPosition in tempPositions)
                    {
                        int index = GetIndex(tempPosition);
                        if (index >= 0 && newPixels[index] == startColor)
                        {
                            if (newPixels[index] == startColor)
                            {
                                pixelsToProcess.AddLast(tempPosition);
                                newPixels[index] = color;
                            }
                        }
                    }

                    pixelsToProcess.RemoveFirst();
                }
            }
        }
    }

    public Color GetColor(Vector3 position)
    {
        var textureCoordinates = GetTextureCoordinates(position);
        return texture.GetPixel(textureCoordinates.x, textureCoordinates.y);
    }

    private void DrawLineInternal(Int2 start, Int2 end, List<Int2> points)
    {
        canvasLineDrawer.Reset(start, end);

        Int2 linePosition = start;
        do
        {
            points.Add(linePosition);
        }
        while (canvasLineDrawer.GetNext(ref linePosition));
    }

    private void DrawPoints(List<Int2> points, Color color)
    {
        foreach (var point in points)
        {
            int index = GetIndex(point);
            if (index >= 0 && index < texWidth * texHeight)
                newPixels[index] = color;
        }

        if (subTiles == null)
        {
            Texture.SetPixels(newPixels);
            Texture.Apply();
        }
        else
        {
            int width = texWidth / GlobalData.spritePixelSize;

            foreach (var point in points)
            {
                int index = GetIndex(point);

                if (index >= 0 && index < texWidth * texHeight)
                {
                    int subTileX = point.x / GlobalData.spritePixelSize;
                    int subTileY = point.y / GlobalData.spritePixelSize;

                    var subTileIndex = subTileX + subTileY * width;

                    if (subTileIndex >= 0 && subTileIndex < subTiles.Length)
                        subTiles[subTileIndex].SetPixel(point.x % GlobalData.spritePixelSize, point.y % GlobalData.spritePixelSize, color);
                }
            }

            foreach (var subTile in subTiles)
                subTile.Apply();
        }
    }

    private Int2 GetTextureCoordinates(Vector3 worldPosition)
    {
        float fx = ((worldPosition.x - transform.position.x) - bottomLeft.x) / (topRight.x - bottomLeft.x);
        float fy = ((worldPosition.y - transform.position.y) - bottomLeft.y) / (topRight.y - bottomLeft.y);

        return new Int2((int)Mathf.Floor(fx * (float)texWidth), (int)Mathf.Floor(fy * (float)texHeight));
    }

    private int GetIndex(Int2 position)
    {
        if (position.x < 0 || position.x >= texWidth || position.y < 0 || position.y >= texHeight)
            return -1;

        return position.x + position.y * texWidth;
    }

    private void ClearSubTiles()
    {
        if (subTiles != null)
        {
            foreach (var subTile in subTiles)
                subTile.Clear();
        }
    }

    private class CanvasLineDrawer
    {
        private Int2 drawLineStart;
        private Int2 drawLineEnd;
        private Int2 drawLineD;
        private Int2 drawLineS;
        private Int2 drawLine;
        private int iDrawLineErr;

        public void Reset(Int2 start, Int2 end)
        {
            drawLineStart = start;
            drawLineEnd = end;
            drawLineD = new Int2(Mathf.Abs(drawLineEnd.x - drawLineStart.x), Mathf.Abs(drawLineEnd.y - drawLineStart.y));

            if (drawLineStart.x < drawLineEnd.x)
            {
                drawLineS.x = 1;
            }
            else
            {
                drawLineS.x = -1;
            }
            if (drawLineStart.y < drawLineEnd.y)
            {
                drawLineS.y = 1;
            }
            else
            {
                drawLineS.y = -1;
            }
            iDrawLineErr = drawLineD.x - drawLineD.y;
            drawLine = drawLineStart;
        }
        public bool GetNext(ref Int2 position)
        {
            position = drawLine;
            if (drawLine == drawLineEnd)
            {
                return false;
            }
            int e2 = 2 * iDrawLineErr;
            if (e2 > -drawLineD.y)
            {
                iDrawLineErr -= drawLineD.y;
                drawLine.x += drawLineS.x;
            }
            if (e2 < drawLineD.x)
            {
                iDrawLineErr += drawLineD.x;
                drawLine.y += drawLineS.y;
            }
            return true;
        }
    }
}