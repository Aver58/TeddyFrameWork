﻿#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Versions.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 21:50:59
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

	public static AssetBundle LoadAssetBundleFromFile(string url)
	{
		if(!File.Exists(url))
		{
			if(_disk != null)
			{
				var name = Path.GetFileName(url);
				var file = _disk.GetFile(name, string.Empty);
				if(file != null)
				{
					return AssetBundle.LoadFromFile(_disk.name, 0, (ulong)file.offset);
				}
			}
		}
		return AssetBundle.LoadFromFile(url);
	}

	public static AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string url)
	{
		if(!File.Exists(url))
		{
			if(_disk != null)
			{
				var name = Path.GetFileName(url);
				var file = _disk.GetFile(name, string.Empty);
				if(file != null)
				{
					return AssetBundle.LoadFromFileAsync(_disk.name, 0, (ulong)file.offset);
				}
			}
		}
		return AssetBundle.LoadFromFileAsync(url);
	}

	public static void BuildVersions(string outputPath, string[] bundles, int version)
	{
		var path = outputPath + "/" + Filename;
		if(File.Exists(path))
			File.Delete(path);

		var dataPath = outputPath + "/" + Dataname;
		if(File.Exists(dataPath))
			File.Delete(dataPath);

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

	public static int LoadVersion(string filename)
	{
		if(!File.Exists(filename))
			return -1;
		try
		{
			using(var stream = File.OpenRead(filename))
			{
				var reader = new BinaryReader(stream);
				return reader.ReadInt32();
			}
		}
		catch(Exception e)
		{
			GameLog.LogException(e);
			return -1;
		}
	}

	public static List<VFile> LoadVersions(string filename, bool update = false)
	{
		string rootDir = Path.GetDirectoryName(filename);
		Dictionary<string, VFile> data = update ? _updateData : _baseData;
		data.Clear();
		using(FileStream stream = File.OpenRead(filename))
		{
			var reader = new BinaryReader(stream);
			var list = new List<VFile>();
			var ver = reader.ReadInt32();
			GameLog.Log("LoadVersions:" + ver);
			int count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var version = new VFile();
				version.Deserialize(reader);
				list.Add(version);
				data[version.name] = version;
				var dir = string.Format("{0}/{1}", rootDir, Path.GetDirectoryName(version.name));
				if(!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
			}
			return list;
		}
	}

	public static bool LoadDisk(string filename)
	{
		return _disk.Load(filename);
	}

	public static bool IsNew(string path, long len, string hash)
	{
		VFile file;
		var key = Path.GetFileName(path);
		if(_baseData.TryGetValue(key, out file))
		{
			if(key.Equals(Dataname) || file.len == len && file.hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
				return false;
		}

		if(_disk.Exists())
		{
			VFile vdf = _disk.GetFile(path, hash);
			if(vdf != null && vdf.len == len && vdf.hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
				return false;
		}

		if(!File.Exists(path))
			return true;

		using(var stream = File.OpenRead(path))
		{
			if(stream.Length != len)
				return true;
			if(verifyBy != VerifyBy.Hash)
				return false;

			return !Utility.GetCRC32Hash(stream).Equals(hash, StringComparison.OrdinalIgnoreCase);
		}
	}

	public static void UpdateDisk(string savePath, List<VFile> newFiles)
	{
		var saveFiles = new List<VFile>();
		var files = _disk.files;
		foreach(VFile file in files)
		{
			if(_updateData.ContainsKey(file.name))
				saveFiles.Add(file);
		}
		_disk.Update(savePath, newFiles, saveFiles);
	}
}