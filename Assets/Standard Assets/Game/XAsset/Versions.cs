#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Versions.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 21:50:59
=====================================================
*/
#endregion

using System.Collections.Generic;
using System.IO;
using libx;

public enum VerifyBy
{
    Size,
    Hash,
}

public static class Versions
{
    public const string Dataname = "res";
    public const string Filename = "ver";
    public static readonly VerifyBy verifyBy = VerifyBy.Hash;
    private static readonly VDisk _disk = new VDisk();
    private static readonly Dictionary<string, VFile> _updateData = new Dictionary<string, VFile>();
    private static readonly Dictionary<string, VFile> _baseData = new Dictionary<string, VFile>();

	public static void BuildVersions(string outputPath, string[] bundles, int version)
	{
		var path = outputPath + "/" + Filename;
		if(File.Exists(path))
		{
			File.Delete(path);
		}
		var dataPath = outputPath + "/" + Dataname;
		if(File.Exists(dataPath))
		{
			File.Delete(dataPath);
		}

		var disk = new VDisk();
		foreach(var file in bundles)
		{
			using(var fs = File.OpenRead(outputPath + "/" + file))
			{
				disk.AddFile(file, fs.Length, Utility.GetCRC32Hash(fs));
			}
		}

		disk.name = dataPath;
		disk.Save();

		using(var stream = File.OpenWrite(path))
		{
			var writer = new BinaryWriter(stream);
			writer.Write(version);
			writer.Write(disk.files.Count + 1);
			using(var fs = File.OpenRead(dataPath))
			{
				var file = new VFile { name = Dataname, len = fs.Length, hash = Utility.GetCRC32Hash(fs) };
				file.Serialize(writer);
			}
			foreach(var file in disk.files)
			{
				file.Serialize(writer);
			}
		}
	}
}