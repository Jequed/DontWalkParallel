using System.IO;

public static class MapIO
{
    private const int VERSION = 2;

    public static void SaveMap(string path, MapContainer mapContainer)
    {
        var writer = new BinaryWriter(File.OpenWrite(path));

        writer.Write(VERSION);

        writer.Write(mapContainer.Levels.Count);
        foreach (var level in mapContainer.Levels)
            SaveLevel(level, writer);

        writer.Write(mapContainer.Connections.Count);
        foreach (var connection in mapContainer.Connections)
            SaveConnection(connection, mapContainer, writer);

        //var pixels = mapContainer.Canvas.Texture.EncodeToPNG();
        //writer.Write(pixels.Length);
        //writer.Write(pixels);

        //writer.Close();
    }

    private static void SaveLevel(MapLevel level, BinaryWriter writer)
    {
        writer.Write(level.LevelName);

        writer.Write(level.Difficulty);

        IOUtility.WriteVector3(level.transform.localPosition, writer);
    }
    private static void SaveConnection(LevelConnection connection, MapContainer mapContainer, BinaryWriter writer)
    {
        writer.Write(mapContainer.Levels.IndexOf(connection.FromLevel));
        writer.Write(mapContainer.Levels.IndexOf(connection.ToLevel));
    }

    public static void OpenMap(string path, MapContainer mapContainer)
    {
        var reader = new BinaryReader(File.OpenRead(path));

        var fileVersion = reader.ReadInt32();

        int levelCount = reader.ReadInt32();
        for (int i = 0; i < levelCount; i++)
           OpenLevel(mapContainer.AddLevel(), reader);

        if (fileVersion >= 2)
        {
            int connectionCount = reader.ReadInt32();
            for (int i = 0; i < connectionCount; i++)
                OpenConnection(mapContainer, reader);

            int pixelCount = reader.ReadInt32();
            var pixels = reader.ReadBytes(pixelCount);
            //mapContainer.Canvas.Texture.LoadImage(pixels);
        }

        reader.Close();
    }

    private static void OpenLevel(MapLevel level, BinaryReader reader)
    {
        level.LevelName = reader.ReadString();

        level.Difficulty = reader.ReadInt32();

        level.transform.localPosition = IOUtility.ReadVector3(reader);
    }
    private static void OpenConnection(MapContainer mapContainer, BinaryReader reader)
    {
        var fromLevel = mapContainer.Levels[reader.ReadInt32()];
        var toLevel = mapContainer.Levels[reader.ReadInt32()];

        mapContainer.AddConnection(fromLevel, toLevel);
    }
}