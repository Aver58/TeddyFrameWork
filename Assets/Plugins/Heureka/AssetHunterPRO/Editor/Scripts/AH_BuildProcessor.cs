
namespace HeurekaGames.AssetHunterPRO
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
    using UnityEditor.Build;

    class AH_BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IProcessSceneWithReport
    {
        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            //For some reason I have to do both recursive, and non-recursive version
            string[] dependencies = AssetDatabase.GetDependencies(scene.path, true);
            dependencies.ToList().AddRange(AssetDatabase.GetDependencies(scene.path, false));
            {
                foreach (string dependency in dependencies)
                    processUsedAsset(scene.path, dependency);
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            initBuildReport(report.summary.platform, report.summary.outputPath);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            finalizeBuildReport(report);
        }

        private void finalizeBuildReport(BuildReport report)
        {
            addBuildReportInfo(report);

            //Dont force add special folders (resources etc) in 2018.1 because we have asccess to buildreport
            finalizeBuildReport(report.summary.platform);
        }

        private void addBuildReportInfo(BuildReport report)
        {
            if(buildInfo != null)
                buildInfo.ProcessBuildReport(report);
        }

#elif UNITY_5_6_OR_NEWER
    using UnityEditor.Build;

    class AH_BuildProcessor : IProcessScene, IPreprocessBuild, IPostprocessBuild
    {
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            initBuildReport(target, path);
        }

        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene)
        {
            Debug.Log("AH: Processing scene: " + scene.name);
            string[] dependencies = AssetDatabase.GetDependencies(scene.path, true);
            {             
                foreach (string dependency in dependencies)
                    processUsedAsset(scene.path, dependency);
            }
        }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            finalizeBuildReport(target);
        }
#endif
        //#if UNITY_5_6_OR_NEWER
        static AH_SerializedBuildInfo buildInfo;

        private bool isProcessing;
        private static bool isGenerating;

        private void initBuildReport(BuildTarget platform, string outputPath)
        {
            //Only start processing if its set in preferences
            isProcessing = AH_SettingsManager.Instance.AutoCreateLog || isGenerating;

            if (isProcessing)
            {
                Debug.Log("AH: Initiated new buildreport - " + System.DateTime.Now);
                buildInfo = new AH_SerializedBuildInfo();
            }
            else
            {
                Debug.Log("AH: Build logging not automatically started. Open Asset Hunter preferences if you want it to run");
            }
        }

        private void finalizeBuildReport(BuildTarget target)
        {
            if (isProcessing)
            {
                isProcessing = false;

                Debug.Log("AH: Finalizing new build report - " + System.DateTime.Now);

                buildInfo.FinalizeReport(target);
            }
        }

        internal static void GenerateBuild()
        {
            isGenerating = true;

            string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
            BuildPlayerOptions bpOptions = new BuildPlayerOptions();
            bpOptions.locationPathName = path + "/AH_GeneratedBuild.exe";
            bpOptions.scenes = EditorBuildSettings.scenes.Where(val => val.enabled).Select(val => val.path).ToArray<string>();
            bpOptions.target = EditorUserBuildSettings.activeBuildTarget;
            bpOptions.targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            bpOptions.options = BuildOptions.None;

            // Build player.
            BuildPipeline.BuildPlayer(bpOptions);
        }

        private void processUsedAsset(string scenePath, string assetPath)
        {
            if (isProcessing)
                buildInfo.AddBuildDependency(scenePath, assetPath);
        }

        public int callbackOrder { get { return 0; } }
    }
    //#endif
}