using System.IO;
using UnityEngine;

public static class FileUtility
{
	public static void WriteVector3(BinaryWriter writer, Vector3 vec)
	{
		writer.Write(vec.x);
		writer.Write(vec.y);
		writer.Write(vec.z);
	}
	public static Vector3 ReadVector3(BinaryReader reader)
	{
		Vector3 vec;
		vec.x = reader.ReadSingle();
		vec.y = reader.ReadSingle();
		vec.z = reader.ReadSingle();
		return vec;
	}
}