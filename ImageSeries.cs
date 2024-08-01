using UnityEngine;

public class ImageSeries
{
    public bool flipX;
    public bool flipY;

    public Sprite[] sprites;

    public float speed = 1.0f;

    public ImageSeries(ImageSeriesData data, Sprite[] sprites)
    {
        flipX = data.flipX;
        flipY = data.flipY;
        speed = data.speed;

        string[] indices = data.pattern.Split(',');
        this.sprites = new Sprite[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            this.sprites[i] = sprites[int.Parse(indices[i])];
    }
}