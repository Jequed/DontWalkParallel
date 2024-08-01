using System.Collections.Generic;
using UnityEngine;

public class MapContainer : MonoBehaviour
{
    [SerializeField]
    private DWPCanvas mapCanvas;

    private List<MapLevel> levels = new List<MapLevel>();
    private List<LevelConnection> connections = new List<LevelConnection>();

    public List<MapLevel> Levels
    {
        get
        {
            return levels;
        }
    }
    public List<LevelConnection> Connections
    {
        get
        {
            return connections;
        }
    }
    public DWPCanvas Canvas
    {
        get
        {
            if (!mapCanvas.Inititalized)
                InitializeCanvas();

            return mapCanvas;
        }
    }

    private void InitializeCanvas()
    {
        const float downsize = 10.0f;
        const int texResolution = 2;

        mapCanvas.Initialize(
            1920.0f / downsize, 1080.0f / downsize,
            1920 * texResolution, 1080 * texResolution);
    }

    public MapLevel AddLevel()
    {
        var mapLevel = Instantiate(Resources.Load<MapLevel>("Prefabs/EditorPrefabs/MapLevel"));
        mapLevel.transform.parent = transform;
        levels.Add(mapLevel);

        return mapLevel;
    }

    public LevelConnection AddConnection(MapLevel fromLevel, MapLevel toLevel)
    {
        foreach (var existingConnection in connections)
        {
            if ((existingConnection.FromLevel == fromLevel && existingConnection.ToLevel == toLevel) || (existingConnection.FromLevel == toLevel && existingConnection.ToLevel == fromLevel))
                return null;
        }

        var connection = Instantiate(Resources.Load<LevelConnection>("Prefabs/EditorPrefabs/LevelConnection"));
        connection.transform.parent = transform;
        connections.Add(connection);

        connection.FromLevel = fromLevel;
        connection.ToLevel = toLevel;

        return connection;
    }
}