using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelCanvasEditor
{
    public event Action OnColorChanged;

    public enum ToolType
    {
        paintBrush,
        paintBucket,
        inkDropper,
        line
    }

    private Editor editor;

    private DWPCanvas currentCanvas;

    private UndoRedoManager undoRedoManager;

    private bool mouseDown = false;
    private Vector3 mouseStart;

    private ToolType tool = ToolType.paintBrush;

    private Color[] previousColors;

    private Color primaryColor = Color.black;
    private Color secondaryColor = Color.clear;

    private int brushSize = 2;

    private LineDrawer.LineCollection lineToolLines;

    private LineDrawer.LineCollection draw_InitialBrushLines;
    private LineDrawer.LineCollection draw_ReferenceBrushLines;
    private List<Vector3> initialBrushLines = new List<Vector3>();
    private List<Vector3> finalBrushLines = new List<Vector3>();
    private Color finalBrushColor = Color.black;

    private int cachedSubTileWidth = 0;
    private int cachedSubTileHeight = 0;
    private CanvasSubTile[] subTiles;

    private float maxJitterSpread = 0.01f;
    private float jitterSampleDistance = 0.1f;

    private float lineMinForceLength = 0.1f;
    private float lineMaxForceLength = 1.0f;
    private float lineForceSize = 0.001f;
    private float lineMaxSize = 0.2f;

    private bool showJitterLines = false;

    public DWPCanvas CurrentCanvas
    {
        get
        {
            return currentCanvas;
        }
        set
        {
            var oldCanvas = currentCanvas;

            currentCanvas = value;

            if (currentCanvas != oldCanvas && currentCanvas.Inititalized)
                ResetPreviousColors();
        }
    }

    public ToolType Tool
    {
        get
        {
            return tool;
        }
        set
        {
            tool = value;
        }
    }

    public Color PrimaryColor
    {
        get
        {
            return primaryColor;
        }
        set
        {
            primaryColor = value;
            if (OnColorChanged != null)
                OnColorChanged();
        }
    }
    public Color SecondaryColor
    {
        get
        {
            return secondaryColor;
        }
        set
        {
            secondaryColor = value;
            if (OnColorChanged != null)
                OnColorChanged();
        }
    }
    public int BrushSize
    {
        get
        {
            return brushSize;
        }
        set
        {
            brushSize = value;
        }
    }

    public float LineMinForceLength
    {
        get
        {
            return lineMinForceLength;
        }
        set
        {
            lineMinForceLength = value;
        }
    }
    public float LineMaxForceLength
    {
        get
        {
            return lineMaxForceLength;
        }
        set
        {
            lineMaxForceLength = value;
        }
    }
    public float LineForceSize
    {
        get
        {
            return lineForceSize;
        }
        set
        {
            lineForceSize = value;
        }
    }
    public float LineMaxSize
    {
        get
        {
            return lineMaxSize;
        }
        set
        {
            lineMaxSize = value;
        }
    }

    public float MaxJitterSpread
    {
        get
        {
            return maxJitterSpread;
        }
        set
        {
            maxJitterSpread = value;
        }
    }
    public float JitterSampleDistance
    {
        get
        {
            return jitterSampleDistance;
        }
        set
        {
            jitterSampleDistance = value;
        }
    }

    public LevelCanvasEditor(Editor editor)
    {
        this.editor = editor;

        currentCanvas = editor.LevelContainer.Canvas_Background;

        undoRedoManager = new UndoRedoManager();

        lineToolLines = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(Color.blue));

        draw_InitialBrushLines = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(Color.red));
        draw_ReferenceBrushLines = editor.LineDrawer.AddCollection(new LineDrawer.LineCollection(Color.green));
    }

    public void Update()
    {
        lineToolLines.Clear();

        if (currentCanvas.gameObject.activeSelf && currentCanvas.Inititalized)
        {
            int canvasWidth = currentCanvas.Texture.width / GlobalData.spritePixelSize;
            int canvasHeight = currentCanvas.Texture.height / GlobalData.spritePixelSize;

            if (canvasWidth != cachedSubTileWidth || canvasHeight != cachedSubTileHeight)
            {
                cachedSubTileWidth = canvasWidth;
                cachedSubTileHeight = canvasHeight;

                RecalculatedSubTiles();
            }


            bool leftMouse = InputUtility.LeftMouse();
            bool rightMouse = InputUtility.RightMouse();

            if (!editor.MouseInEditor())
            {
                if (leftMouse || rightMouse)
                {
                    bool isPrimary = true;
                    if (rightMouse || editor.DeleteMode)
                        isPrimary = false;

                    var color = isPrimary ? primaryColor : secondaryColor;

                    if (!mouseDown)
                    {
                        BeginDrawing();
                        initialBrushLines.Add(editor.MouseWorldPosition);
                        finalBrushColor = color;
                    }

                    switch (tool)
                    {
                        case ToolType.paintBrush:
                            if (editor.PreviousMouseWorldPosition != editor.MouseWorldPosition)
                            {
                                currentCanvas.DrawLine(editor.PreviousMouseWorldPosition, editor.MouseWorldPosition, color, brushSize);
                                if (showJitterLines)
                                {
                                    draw_InitialBrushLines.AddPoint(editor.PreviousMouseWorldPosition);
                                    draw_InitialBrushLines.AddPoint(editor.MouseWorldPosition);
                                }
                                initialBrushLines.Add(editor.MouseWorldPosition);
                            }
                            break;
                        case ToolType.paintBucket:
                            currentCanvas.Fill(editor.MouseWorldPosition, color);
                            break;
                        case ToolType.inkDropper:
                            if (isPrimary)
                                PrimaryColor = currentCanvas.GetColor(editor.MouseWorldPosition);
                            else
                                SecondaryColor = currentCanvas.GetColor(editor.MouseWorldPosition);
                            break;
                        case ToolType.line:
                            lineToolLines.Color = color;
                            lineToolLines.AddPoint(mouseStart);
                            lineToolLines.AddPoint(editor.MouseWorldPosition);
                            break;
                    }

                    mouseDown = true;
                }
            }

            if (mouseDown && !leftMouse && !rightMouse)
            {
                mouseDown = false;
                EndDrawing();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
            {
                ImportImage("C:\\Users\\Brandon\\Documents\\DontWalkParallel\\Assets\\Resources\\Images\\Backgrounds\\EZ Building\\Hanging out after class.png");
            }
        }
    }

    public void ResetPreviousColors()
    {
        if (currentCanvas.Inititalized)
            previousColors = currentCanvas.Texture.GetPixels();
    }

    private void BeginDrawing()
    {
        bool useSubTiles = (tool == ToolType.paintBrush);

        currentCanvas.BeginDrawing(useSubTiles ? subTiles : null);
        editor.LevelContainer.CanvasSubTiles.gameObject.SetActive(useSubTiles);

        mouseStart = editor.MouseWorldPosition;

        draw_InitialBrushLines.Clear();
        initialBrushLines.Clear();
    }
    private void EndDrawing()
    {
        if (tool == ToolType.line)
            DrawImperfectLine(mouseStart, editor.MouseWorldPosition, lineToolLines.Color, brushSize);

        if (tool == ToolType.paintBrush)
        {
            CorrectJitter();
            currentCanvas.EndDrawing(finalBrushLines.ToArray(), finalBrushColor, brushSize);
        }
        else
        {
            currentCanvas.EndDrawing();
        }

        Commit();
        editor.LevelContainer.CanvasSubTiles.gameObject.SetActive(false);
    }

    private void RecalculatedSubTiles()
    {
        subTiles = new CanvasSubTile[cachedSubTileWidth * cachedSubTileHeight];

        Int2 position = new Int2(0, 0);
        for (int i = 0; i < subTiles.Length; i++)
        {
            subTiles[i] = GameObject.Instantiate(Resources.Load<CanvasSubTile>("Prefabs/EditorPrefabs/CanvasSubTile"));

            subTiles[i].transform.parent = editor.LevelContainer.CanvasSubTiles.transform;

            subTiles[i].transform.localPosition = new Vector3(currentCanvas.transform.localPosition.x - currentCanvas.Scale.x * 0.5f + 0.5f + (float)position.x, currentCanvas.transform.localPosition.y - currentCanvas.Scale.y * 0.5f + 0.5f + (float)position.y, subTiles[i].transform.position.z);

            position.x++;
            if (position.x >= cachedSubTileWidth)
            {
                position.x = 0;
                position.y++;
            }
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void DrawImperfectLine(Vector3 lineStart, Vector3 lineEnd, Color color, int brushSize)
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 lineSub = lineEnd - lineStart;
        Vector3 lineVec = lineSub.normalized;
        Vector3 lineNormal = new Vector3(-lineVec.y, lineVec.x, lineVec.z);
        float lineMagnitude = lineSub.magnitude;

        const float stepLength = 1.0f / GlobalData.spritePixelSize;

        float elapsedLength = 0.0f;
        Vector3 currentPosition = lineStart;

        float currentForce = MathUtility.GetRandomRange(-lineForceSize, lineForceSize);
        float elapsedForce = 0.0f;
        float lengthToNextForce = MathUtility.GetRandomRange(lineMinForceLength, lineMaxForceLength);

        while (elapsedLength < lineMagnitude)
        {
            elapsedLength += stepLength;
            elapsedForce += stepLength;

            currentPosition += lineVec * stepLength + lineNormal * currentForce;

            if (elapsedForce > lengthToNextForce)
            {
                elapsedForce = 0.0f;

                lengthToNextForce = MathUtility.GetRandomRange(lineMinForceLength, lineMaxForceLength);

                currentForce = MathUtility.GetRandomRange(-lineForceSize, lineForceSize);

                var sub = currentPosition - lineStart;
                if (Vector3.Dot(sub, currentForce * lineNormal) > 0.0f)
                {
                    //Try to direct it back to the middle

                    float chanceToChangeDirection = (sub - Vector3.Project(sub, lineVec)).magnitude / lineMaxSize;

                    if (MathUtility.GetRandomRange(0.0f, 1.0f) < chanceToChangeDirection)
                        currentForce *= -1.0f;
                }
            }

            points.Add(currentPosition);
        }

        currentCanvas.DrawLines(points.ToArray(), color, brushSize);
    }

    private void CorrectJitter()
    {
        if (initialBrushLines.Count > 0)
        {
            finalBrushLines.Clear();
            finalBrushLines.Add(initialBrushLines[0]);

            int lastPoint = 0;
            float currentDistance = 0.0f;

            List<Vector3> referenceLine = new List<Vector3>();
            referenceLine.Add(initialBrushLines[0]);
            for (int i = 1; i < initialBrushLines.Count; i++)
            {
                Vector3 point1 = initialBrushLines[i - 1];
                Vector3 point2 = initialBrushLines[i];

                float distanceToAdd = Vector3.Distance(point1, point2);

                if (currentDistance + distanceToAdd > jitterSampleDistance)
                {
                    Vector3 vec = (point2 - point1).normalized;

                    Vector3 lastPosition = point1 + (jitterSampleDistance - currentDistance) * vec;

                    for (int j = lastPoint; j < i; j++)
                        finalBrushLines.Add(FixJitter(referenceLine[referenceLine.Count - 1], lastPosition, initialBrushLines[j]));

                    referenceLine.Add(lastPosition);
                    lastPoint = i;
                    currentDistance = 0.0f;

                    while (Vector3.Distance(lastPosition, point2) > jitterSampleDistance)
                    {
                        lastPosition += jitterSampleDistance * vec;
                        referenceLine.Add(lastPosition);
                        currentDistance = 0.0f;
                    }

                    currentDistance += Vector3.Distance(lastPosition, point2);
                }
                else
                {
                    currentDistance += distanceToAdd;
                }
            }

            for (int j = lastPoint; j < initialBrushLines.Count; j++)
                finalBrushLines.Add(FixJitter(referenceLine[referenceLine.Count - 1], initialBrushLines[initialBrushLines.Count - 1], initialBrushLines[j]));

            referenceLine.Add(initialBrushLines[initialBrushLines.Count - 1]);

            draw_ReferenceBrushLines.Clear();
            if (showJitterLines)
            {
                for (int i = 1; i < referenceLine.Count; i++)
                {
                    draw_ReferenceBrushLines.AddPoint(referenceLine[i - 1]);
                    draw_ReferenceBrushLines.AddPoint(referenceLine[i]);
                }
            }
        }
    }
    private Vector3 FixJitter(Vector3 line1, Vector3 line2, Vector3 point)
    {
        Vector3 newPoint = point;

        Vector3 lineSub = line2 - line1;
        Vector3 pointSub = point - line1;
        Vector3 projectedSub = Vector3.Project(pointSub, lineSub);

        Vector3 diff = pointSub - projectedSub;
        if (diff.magnitude < maxJitterSpread)
            newPoint = line1 + projectedSub;// + (pointSub - projectedSub).normalized * maxJitterSpread;

        return newPoint;
    }

    private void ImportImage(string imageName)
    {
        var reader = new BinaryReader(File.OpenRead(imageName));

        var bytes = reader.ReadBytes((int)reader.BaseStream.Length);

        currentCanvas.Texture.LoadImage(bytes);
        currentCanvas.Texture.Apply();

        reader.Close();
    }

    public void Undo()
    {
        undoRedoManager.Undo();
        ResetPreviousColors();
    }
    public void Redo()
    {
        undoRedoManager.Redo();
        ResetPreviousColors();
    }
    public void Commit()
    {
        Color[] currentColors = currentCanvas.Texture.GetPixels();

        var pixelsChanged = new LinkedList<CanvasUndoable.CanvasUndoablePixel>();
        for (int i = 0; i < currentColors.Length; i++)
        {
            if (currentColors[i] != previousColors[i])
                pixelsChanged.AddLast(new CanvasUndoable.CanvasUndoablePixel(previousColors[i], currentColors[i], i));
        }

        if (pixelsChanged.Count > 0)
        {
            undoRedoManager.AddUndoable(new CanvasUndoable(currentCanvas, pixelsChanged));

            undoRedoManager.Commit();

            ResetPreviousColors();
        }
    }
}