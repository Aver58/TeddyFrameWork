using System;
using System.IO;
using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadBuildHelper {
        /// <summary>
        /// Should HotReload runtime be included in the current build?
        /// </summary>
        public static bool IncludeInThisBuild() {
            return IsAllBuildSettingsSupported();
        }

        /// <summary>
        /// Get scripting backend for the current platform.
        /// </summary>
        /// <returns>Scripting backend</returns>
        public static ScriptingImplementation GetCurrentScriptingBackend() {
#pragma warning disable CS0618
            return PlayerSettings.GetScriptingBackend(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
#pragma warning restore CS0618
        }

        public static ManagedStrippingLevel GetCurrentStrippingLevel() {
#pragma warning disable CS0618
            return PlayerSettings.GetManagedStrippingLevel(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
#pragma warning restore CS0618
        }

        public static void SetCurrentScriptingBackend(ScriptingImplementation to) {
#pragma warning disable CS0618
            // only set it if default is not correct (avoid changing ProjectSettings when not needed)
            if (GetCurrentScriptingBackend() != to) {
                PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, to);
            }
#pragma warning restore CS0618
        }
        
        public static void SetCurrentStrippingLevel(ManagedStrippingLevel to) {
#pragma warning disable CS0618
            // only set it if default is not correct (avoid changing ProjectSettings when not needed)
            if (GetCurrentStrippingLevel() != to) {
                PlayerSettings.SetManagedStrippingLevel(EditorUserBuildSettings.selectedBuildTargetGroup, to);
            }
#pragma warning restore CS0618
        }

        /// Is the current build target supported?
        /// main thread only
        public static bool IsBuildTargetSupported() {
            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;  
            return Array.IndexOf(unsupportedBuildTargets, buildTarget) == -1;
        }
        
        /// Are all the settings supported?
        /// main thread only
        static bool IsAllBuildSettingsSupported() {
            if (!IsBuildTargetSupported()) {
                return false;
            }

            // need way to give it settings object, dont want to give serializedobject
            var options = HotReloadSettingsEditor.LoadSettingsOrDefault();
            var so = new SerializedObject(options);
            
            // check all projeect options
            foreach (var option in HotReloadSettingsTab.allOptions) {
                var projectOption = option as ProjectOptionBase;
                if (projectOption != null) {
                    // if option is required, build can't use hot reload
                    if (projectOption.IsRequiredForBuild() && !projectOption.GetValue(so)) {
                        return false;
                    }
                }
            }

            return GetCurrentScriptingBackend() == ScriptingImplementation.Mono2x
                && GetCurrentStrippingLevel() == ManagedStrippingLevel.Disabled
                && EditorUserBuildSettings.development;
        }

        /// <summary>
        /// Some platforms are not supported because they don't have Mono scripting backend.
        /// </summary>
        /// <remarks>
        /// Only list the platforms that definately don't have Mono scripting.
        /// </remarks>
        private static readonly BuildTargetGroup[] unsupportedBuildTargets = new [] {
            BuildTargetGroup.iOS, // mono support was removed many years ago
            BuildTargetGroup.WebGL, // has never had mono
        };
        
        public static bool IsMonoSupported(BuildTargetGroup buildTarget) {
            // "When a platform can support both backends, Mono is the default. For more information, see Scripting restrictions."
            // Unity docs https://docs.unity3d.com/Manual/Mono.html (2019.4/2020.3/2021.3)
#pragma warning disable CS0618 // obsolete since 2023
            var defaultScripting = PlayerSettings.GetDefaultScriptingBackend(buildTarget);
#pragma warning restore CS0618
            if (defaultScripting == ScriptingImplementation.Mono2x) {
                return Array.IndexOf(unsupportedBuildTargets, buildTarget) == -1;
            }
            // default scripting was not Mono, so the platform doesn't support Mono at all.
            return false;
        }
    }
}