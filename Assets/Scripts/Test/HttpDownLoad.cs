using System;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class HttpDownLoad {
    public float progress { get; private set; }

    public bool isDone { get; private set; }

    private bool isStop;

    public IEnumerator Start(string url, string filePath, Action callBack = null)
    {
        var headRequest = UnityWebRequest.Head(url);

        yield return headRequest.SendWebRequest();
        
        var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));

        var dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            var fileLength = fs.Length;

            if (fileLength < totalLength)
            {
                fs.Seek(fileLength, SeekOrigin.Begin);

                var request = UnityWebRequest.Get(url);
                request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + totalLength);
                request.SendWebRequest();

                var index = 0;
                while (!request.isDone)
                {
                    if (isStop) break;
                    yield return null;
                    var buff = request.downloadHandler.data;
                    if (buff != null)
                    {
                        var length = buff.Length - index;
                        fs.Write(buff, index, length);
                        index += length;
                        fileLength += length;

                        if (fileLength == totalLength)
                        {
                            progress = 1f;
                        }
                        else
                        {
                            progress = fileLength / (float) totalLength;
                        }
                    }
                }
            }
            else
            {
                progress = 1f;
            }

            fs.Close();
            fs.Dispose();
        }

        if (progress >= 1f)
        {
            isDone = true;
            callBack?.Invoke();
        }
    }

    public void Stop()
    {
        isStop = true;
    }
}
