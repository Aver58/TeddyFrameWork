using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;

namespace HeurekaGames.AssetHunterPRO
{
    [System.Serializable]
    public class AH_BuildReportFileInfo
    {
        public string Path;
        public string Role;
        public ulong Size;

        public AH_BuildReportFileInfo(BuildFile file)
        {
            this.Path = file.path;
            this.Role = file.role;
            this.Size = file.size;
        }
    }
}
#endif