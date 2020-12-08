using System.IO;

public class FileHelper
{
    public static bool IsDirectoryExist(string path)
    {
        return Directory.Exists(path);
    }

    public static bool IsFileExist(string path)
    {
        return File.Exists(path);
    }

    public static void CreateDirectory(string path)
    {
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}