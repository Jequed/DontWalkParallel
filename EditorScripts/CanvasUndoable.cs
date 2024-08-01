using System.Collections.Generic;
using UnityEngine;

public class CanvasUndoable : IUndoable
{
    private DWPCanvas canvas;
    private LinkedList<CanvasUndoablePixel> pixels;

    public struct CanvasUndoablePixel
    {
        public Color colorDelta;
        public int position;

        public CanvasUndoablePixel(Color oldColor, Color newColor, int position)
        {
            colorDelta = newColor - oldColor;
            this.position = position;
        }
    }

    public CanvasUndoable(DWPCanvas canvas, LinkedList<CanvasUndoablePixel> pixels)
    {
        this.canvas = canvas;
        this.pixels = pixels;
    }

    public void Undo()
    {
        var texturePixels = canvas.Texture.GetPixels();

        foreach (var pixel in pixels)
            texturePixels[pixel.position] -= pixel.colorDelta;

        canvas.Texture.SetPixels(texturePixels);
        canvas.Texture.Apply();
    }

    public void Redo()
    {
        var texturePixels = canvas.Texture.GetPixels();

        foreach (var pixel in pixels)
            texturePixels[pixel.position] += pixel.colorDelta;

        canvas.Texture.SetPixels(texturePixels);
        canvas.Texture.Apply();
    }
}