using System.IO;
using UnityEngine;

public static class IOUtility
{
    public static void WriteVector3(Vector3 vec, BinaryWriter writer)
    {
        writer.Write(vec.x);
        writer.Write(vec.y);
        writer.Write(vec.z);
    }
    public static void WriteColor(Color color, BinaryWriter writer)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
        writer.Write(color.a);
    }

    public static Vector3 ReadVector3(BinaryReader reader)
    {
        Vector3 vec;
        vec.x = reader.ReadSingle();
        vec.y = reader.ReadSingle();
        vec.z = reader.ReadSingle();
        return vec;
    }
    public static Color ReadColor(BinaryReader reader)
    {
        Color col;
        col.r = reader.ReadSingle();
        col.g = reader.ReadSingle();
        col.b = reader.ReadSingle();
        col.a = reader.ReadSingle();
        return col;
    }
}