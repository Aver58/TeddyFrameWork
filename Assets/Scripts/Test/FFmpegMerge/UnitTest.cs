using System.IO;
using UnityEngine;

public class UnitTest : MonoBehaviour {
    void Start() {
        var folder = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string[] inputFiles = {
            folder + "\\Captures\\filelist1.mp4",
            folder + "\\Captures\\filelist2.mp4",
            folder + "\\Captures\\filelist3.mp4",
            folder + "\\Captures\\filelist4.mp4",
        };
        var outputFile = folder + "\\Captures\\output.mp4";

        FFMpegHelper.MergeVideos(inputFiles, outputFile);
    }
}
