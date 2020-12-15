using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public static class AH_Utils
    {
        internal static string GetSizeAsString(ulong byteSize)
        {
            return GetSizeAsString((long)byteSize);
        }

        internal static string GetSizeAsString(long byteSize)
        {
            string sizeAsString = String.Empty;

            float b = byteSize;
            float kb = b / 1024f;
            float mb = kb / 1024;
            float gb = mb / 1024;

            if (gb >= 1)
            {
                sizeAsString = String.Format(((float)Math.Round(gb, 1)).ToString(), "0.00") + " gb";
            }
            else if (mb >= 1)
            {
                sizeAsString = String.Format(((float)Math.Round(mb, 1)).ToString(), "0.00") + " mb";
            }
            else if (kb >= 1)
            {
                sizeAsString = String.Format(((float)Math.Round(kb, 1)).ToString(), "0.00") + " kb";
            }
            else if (byteSize >= 0)
            {
                sizeAsString = String.Format(((float)Math.Round(b, 1)).ToString(), "0.00") + " b";
            }
            return sizeAsString;
        }

        internal static void GetRelativePathAndAssetID(string absPath, out string relativePath, out string assetGuid)
        {
            relativePath = FileUtil.GetProjectRelativePath(absPath);
            assetGuid = AssetDatabase.AssetPathToGUID(relativePath);
        }

        public static string[] GetEnabledSceneNamesInBuild()
        {
            return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
        }

        public static string[] GetAllSceneNamesInBuild()
        {
            return (from scene in EditorBuildSettings.scenes select scene.path).ToArray();
        }

        public static string[] GetAllSceneNames()
        {
            return (from scene in AssetDatabase.GetAllAssetPaths() where scene.EndsWith(".unity") select scene).ToArray();
        }

        public static System.String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        internal static void PingObjectAtPath(string assetPath, bool select)
        {
            UnityEngine.Object loadObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(loadObj);

            if (select)
                Selection.activeObject = loadObj;
        }

        /// <summary>
        /// A method to shorten file paths using ... delimiter
        /// </summary>
        /// <param name="absolutepath">The path to compress</param>
        /// <param name="limit">The maximum length</param>
        /// <returns></returns>
        public static string ShrinkPathMiddle(string absolutepath, int limit)
        {
            //no path provided
            if (string.IsNullOrEmpty(absolutepath))
            {
                return "";
            }

            var name = Path.GetFileName(absolutepath);
            int namelen = name.Length;
            int pathlen = absolutepath.Length;
            var dir = absolutepath.Substring(0, pathlen - namelen);
            string delimiter = "…";

            int delimlen = delimiter.Length;
            int idealminlen = namelen + delimlen;

            var slash = (absolutepath.IndexOf("/") > -1 ? "/" : "\\");

            //less than the minimum amt
            if (limit < ((2 * delimlen) + 1))
            {
                return "";
            }

            //fullpath
            if (limit >= pathlen)
            {
                return absolutepath;
            }

            //file name condensing
            if (limit < idealminlen)
            {
                return delimiter + name.Substring(0, (limit - (2 * delimlen))) + delimiter;
            }

            //whole name only, no folder structure shown
            if (limit == idealminlen)
            {
                return delimiter + name;
            }

            return dir.Substring(0, (limit - (idealminlen + 1))) + delimiter + slash + name;
        }

        internal static int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        internal static bool IntToBool(int value)
        {
            return (value != 0) ? true : false;
        }

        public static string ShrinkPathEnd(string absolutepath, int limit)
        {
            //no path provided
            if (string.IsNullOrEmpty(absolutepath))
            {
                return "";
            }

            var name = Path.GetFileName(absolutepath);
            int namelen = name.Length;
            int pathlen = absolutepath.Length;
            string delimiter = "…";

            int delimlen = delimiter.Length;

            var slash = (absolutepath.IndexOf("/") > -1 ? "/" : "\\");

            //filesname longer than limit
            if (namelen >= limit)
            {
                return name;
            }

            //fullpath
            if (limit >= pathlen)
            {
                return absolutepath;
            }

            //Get substring within limit
            var pathWithinLimit = absolutepath.Substring(pathlen - limit, limit);
            int indexOfSlash = pathWithinLimit.IndexOf(slash);
            return delimiter + pathWithinLimit.Substring(indexOfSlash, pathWithinLimit.Length - indexOfSlash);
        }

        internal static List<Texture> GetTargetGroupAssetDependencies(BuildTargetGroup targetGroup)
        {
            List<Texture> buildTargetAssetDependencies = new List<Texture>();

            //Run through icons, splashscreens etc and include them as being used
            Texture2D[] targetGroupIcons = PlayerSettings.GetIconsForTargetGroup(targetGroup);
            List<Texture2D> additionalTargetGroupIcons = getAdditionalTargetAssets(targetGroup);

            Texture2D[] unknownTargetGroupIcons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);

            PlayerSettings.SplashScreenLogo[] splashLogos = PlayerSettings.SplashScreen.logos;

            //Loop default targetgroup icons
            for (int i = 0; i < unknownTargetGroupIcons.Length; i++)
            {
                addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, unknownTargetGroupIcons[i]);
            }
            //Loop targetgroup icons
            for (int i = 0; i < targetGroupIcons.Length; i++)
            {
                addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, targetGroupIcons[i]);
            }
            //Loop additional targetgroup icons
            if (additionalTargetGroupIcons != null)
                for (int i = 0; i < additionalTargetGroupIcons.Count; i++)
                {
                    addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, additionalTargetGroupIcons[i]);
                }
            //Loop splash
            for (int i = 0; i < splashLogos.Length; i++)
            {
                addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, splashLogos[i].logo);
            }

            //Get all the custom playersetting textures
            addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, PlayerSettings.defaultCursor);
            addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, PlayerSettings.virtualRealitySplashScreen);
            addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, PlayerSettings.SplashScreen.background);
            addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, PlayerSettings.SplashScreen.backgroundPortrait);
#if !UNITY_2019_1_OR_NEWER
            addTextureToPlayerSettingsList(ref buildTargetAssetDependencies, PlayerSettings.resolutionDialogBanner);
#endif
            return buildTargetAssetDependencies;
        }

        private static List<Texture2D> getAdditionalTargetAssets(BuildTargetGroup targetGroup)
        {
            switch (targetGroup)
            {
#if !UNITY_2018_3_OR_NEWER
                case BuildTargetGroup.N3DS:
                    {
                        break;
                    }
                case BuildTargetGroup.PSP2:
                    {
                        break;
                    }
                case BuildTargetGroup.Tizen:
                    {
                        break;
                    }
#endif
#if !UNITY_2019_3_OR_NEWER
                case BuildTargetGroup.Facebook:
                    {
                        break;
                    }
#endif
                case BuildTargetGroup.Android:
                    {
                        break;
                    }
                case BuildTargetGroup.iOS:
                    {
                        break;
                    }
                case BuildTargetGroup.PS4:
                    {
                        Debug.Log("AH: Need " + targetGroup + " documentation to add platform specific images and assets");
                        break;
                    }
                case BuildTargetGroup.Standalone:
                    {
                        break;
                    }
                case BuildTargetGroup.Switch:
                    {
                        return PlayerSettings.Switch.icons.ToList();
                    }
                case BuildTargetGroup.tvOS:
                    {
                        break;
                    }
                case BuildTargetGroup.WebGL:
                    {
                        break;
                    }
                case BuildTargetGroup.WSA:
                    {
                        List<Texture2D> textures = new List<Texture2D>();
                        textures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(PlayerSettings.WSA.packageLogo));

                        HashSet<PlayerSettings.WSAImageScale> exceptionScales = new HashSet<PlayerSettings.WSAImageScale>();

                        foreach (PlayerSettings.WSAImageType imageType in Enum.GetValues(typeof(PlayerSettings.WSAImageType)))
                        {
                            foreach (PlayerSettings.WSAImageScale imageScale in Enum.GetValues(typeof(PlayerSettings.WSAImageScale)))
                            {
                                try
                                {
                                    string imagePath = PlayerSettings.WSA.GetVisualAssetsImage(imageType, imageScale);
                                    textures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath));
                                }
                                catch (Exception)
                                {
                                    exceptionScales.Add(imageScale);
                                    //If that scale doesn't apply to the given WSA image type
                                }
                            }
                        }

                        if (exceptionScales.Count >= 1)
                        {
                            string scaleListString = "";

                            foreach (var item in exceptionScales)
                            {
                                scaleListString += item.ToString() + (exceptionScales.ElementAt(exceptionScales.Count - 1) == item ? "":", ");
                            }

                            Debug.Log("GetVisualAssetsImage method missing support for WSA image scale: " + scaleListString);
                        }

                        return textures;
                    }
                case BuildTargetGroup.XboxOne:
                    {
                        Debug.Log("AH: Need " + targetGroup + " documentation to add platform specific images and assets");
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("AH: Targetgroup unknown: " + targetGroup);
                        break;
                    }
            }
            return null;
        }

        private static void addTextureToPlayerSettingsList(ref List<Texture> playerSettingsTextures, Sprite sprite)
        {
            if (sprite != null)
                addTextureToPlayerSettingsList(ref playerSettingsTextures, sprite.texture);
        }

        private static void addTextureToPlayerSettingsList(ref List<Texture> playerSettingsTextures, Texture2D texture)
        {
            if ((texture != null) && AssetDatabase.IsMainAsset(texture))
                playerSettingsTextures.Add(texture);
        }
    }
}