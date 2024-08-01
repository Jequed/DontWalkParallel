using UnityEngine;

[CreateAssetMenu(fileName = "ImageSeries", menuName = "DontWalkParallel/ImageSeries", order = 1)]
public class ImageSeriesData : ScriptableObject
{
    public string pattern;

    public bool flipX;
    public bool flipY;

    public float speed = 1.0f;
}