using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace MTAssets.EasyMinimapSystem.Editor
{
    /*
     * This class is responsible for creating the menu for this asset. 
     */

    public class Menu : MonoBehaviour
    {
        //Right click menu items

        [MenuItem("GameObject/Minimap System/Minimap Camera", false, 30)]
        static void CreateMinimapCamera(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //Get the current center position of camera scene view
            Selection.activeObject = SceneView.currentDrawingSceneView;
            SceneView lastSceneView = SceneView.lastActiveSceneView;
            Camera sceneViewCamera = lastSceneView.camera;

            GameObject minimapCamera = new GameObject("Minimap Camera");
            minimapCamera.transform.position = sceneViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            minimapCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapCamera.AddComponent<MinimapCamera>();
            if (gameObjectOfClick != null)
            {
                minimapCamera.transform.SetParent(gameObjectOfClick.transform);
                minimapCamera.transform.localPosition = Vector3.zero;
                minimapCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapCamera };
        }

        [MenuItem("GameObject/Minimap System/Minimap Scanner", false, 30)]
        static void CreateMinimapScanner(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            GameObject minimapScanner = new GameObject("Minimap Scanner");
            minimapScanner.transform.position = Vector3.zero;
            minimapScanner.AddComponent<MinimapScanner>();
            if (gameObjectOfClick != null)
                minimapScanner.transform.SetParent(gameObjectOfClick.transform);
            Selection.objects = new GameObject[] { minimapScanner };
        }

        [MenuItem("GameObject/Minimap System/Minimap Item", false, 30)]
        static void CreateMinimapItem(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //Get the current center position of camera scene view
            Selection.activeObject = SceneView.currentDrawingSceneView;
            SceneView lastSceneView = SceneView.lastActiveSceneView;
            Camera sceneViewCamera = lastSceneView.camera;

            GameObject minimapItem = new GameObject("Minimap Item");
            minimapItem.transform.position = sceneViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            minimapItem.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapItem.AddComponent<MinimapItem>();
            if (gameObjectOfClick != null)
            {
                minimapItem.transform.SetParent(gameObjectOfClick.transform);
                minimapItem.transform.localPosition = Vector3.zero;
                minimapItem.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapItem };
        }

        [MenuItem("GameObject/Minimap System/Minimap Line", false, 30)]
        static void CreateMinimapLine(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //Get the current center position of camera scene view
            Selection.activeObject = SceneView.currentDrawingSceneView;
            SceneView lastSceneView = SceneView.lastActiveSceneView;
            Camera sceneViewCamera = lastSceneView.camera;

            GameObject minimapLine = new GameObject("Minimap Line");
            minimapLine.transform.position = sceneViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            minimapLine.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapLine.AddComponent<MinimapLine>();
            if (gameObjectOfClick != null)
            {
                minimapLine.transform.SetParent(gameObjectOfClick.transform);
                minimapLine.transform.localPosition = Vector3.zero;
                minimapLine.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapLine };
        }

        [MenuItem("GameObject/Minimap System/Minimap Text", false, 30)]
        static void CreateMinimapText(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //Get the current center position of camera scene view
            Selection.activeObject = SceneView.currentDrawingSceneView;
            SceneView lastSceneView = SceneView.lastActiveSceneView;
            Camera sceneViewCamera = lastSceneView.camera;

            GameObject minimapText = new GameObject("Minimap Text");
            minimapText.transform.position = sceneViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            minimapText.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapText.AddComponent<MinimapText>();
            if (gameObjectOfClick != null)
            {
                minimapText.transform.SetParent(gameObjectOfClick.transform);
                minimapText.transform.localPosition = Vector3.zero;
                minimapText.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapText };
        }

        [MenuItem("GameObject/Minimap System/Minimap Renderer", false, 30)]
        static void CreateMinimapRenderer(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //If has not clicked in a component of canvas
            if (gameObjectOfClick == null || gameObjectOfClick.GetComponent<RectTransform>() == null)
            {
                EditorUtility.DisplayDialog("Error", "Select a GameObject that is inside a Canvas or a Canvas to create a Minimap Renderer.", "Ok");
                return;
            }

            //If has clicked in a component of canvas
            if (gameObjectOfClick.GetComponent<RectTransform>() != null)
            {
                GameObject minimapRenderer = new GameObject("Minimap Renderer");
                RectTransform rect = minimapRenderer.AddComponent<RectTransform>();
                minimapRenderer.AddComponent<MinimapRenderer>();
                if (gameObjectOfClick != null)
                {
                    minimapRenderer.transform.SetParent(gameObjectOfClick.transform);
                    rect.localPosition = Vector3.zero;
                    rect.localRotation = Quaternion.Euler(0, 0, 0);
                }
                Selection.objects = new GameObject[] { minimapRenderer };
            }
        }

        [MenuItem("GameObject/Minimap System/Minimap Compass", false, 30)]
        static void CreateMinimapCompass(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            //If has not clicked in a component of canvas
            if (gameObjectOfClick == null || gameObjectOfClick.GetComponent<RectTransform>() == null)
            {
                EditorUtility.DisplayDialog("Error", "Select a GameObject that is inside a Canvas or a Canvas to create a Minimap Compass.", "Ok");
                return;
            }

            //If has clicked in a component of canvas
            if (gameObjectOfClick.GetComponent<RectTransform>() != null)
            {
                GameObject minimapCompass = new GameObject("Minimap Compass");
                RectTransform rect = minimapCompass.AddComponent<RectTransform>();
                minimapCompass.AddComponent<MinimapCompass>();
                if (gameObjectOfClick != null)
                {
                    minimapCompass.transform.SetParent(gameObjectOfClick.transform);
                    rect.localPosition = Vector3.zero;
                    rect.localRotation = Quaternion.Euler(0, 0, 0);
                    rect.sizeDelta = new Vector2(360, 50);
                }
                Selection.objects = new GameObject[] { minimapCompass };
            }
        }

        [MenuItem("GameObject/Minimap System/Minimap Fog", false, 30)]
        static void CreateMinimapFog(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            GameObject minimapFog = new GameObject("Minimap Fog");
            minimapFog.transform.position = new Vector3(0, 0, 0);
            minimapFog.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapFog.AddComponent<MinimapFog>();
            if (gameObjectOfClick != null)
            {
                minimapFog.transform.SetParent(gameObjectOfClick.transform);
                minimapFog.transform.localPosition = Vector3.zero;
                minimapFog.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapFog };
        }

        [MenuItem("GameObject/Minimap System/Minimap Routes", false, 30)]
        static void CreateMinimapRoutes(MenuCommand command)
        {
            //Get the gameobject of right click
            GameObject gameObjectOfClick = (GameObject)command.context;

            GameObject minimapRoutes = new GameObject("Minimap Routes");
            minimapRoutes.transform.position = new Vector3(0, 0, 0);
            minimapRoutes.transform.rotation = Quaternion.Euler(0, 0, 0);
            minimapRoutes.AddComponent<MinimapRoutes>();
            if (gameObjectOfClick != null)
            {
                minimapRoutes.transform.SetParent(gameObjectOfClick.transform);
                minimapRoutes.transform.localPosition = Vector3.zero;
                minimapRoutes.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            Selection.objects = new GameObject[] { minimapRoutes };
        }

        //Menu items

        [MenuItem("Tools/MT Assets/Easy Minimap System/View Changelog", false, 10)]
        static void OpenChangeLog()
        {
            string filePath = Greetings.pathForThisAsset + "/List Of Changes.txt";

            if (File.Exists(filePath) == true)
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)));

            if (File.Exists(filePath) == false)
                EditorUtility.DisplayDialog(
                    "Error",
                    "Unable to open file. The file has been deleted, or moved. Please, to correct this problem and avoid future problems with this tool, remove the directory from this asset and install it again.",
                    "Ok");
        }

        [MenuItem("Tools/MT Assets/Easy Minimap System/Read Documentation", false, 30)]
        static void ReadDocumentation()
        {
            EditorUtility.DisplayDialog(
                  "Read Documentation",
                  "The Documentation HTML file will open in your default application.",
                  "Cool!");

            string filePath = Greetings.pathForThisAsset + Greetings.pathForThisAssetDocumentation;

            if (File.Exists(filePath) == true)
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)));

            if (File.Exists(filePath) == false)
                EditorUtility.DisplayDialog(
                    "Error",
                    "Unable to open file. The file has been deleted, or moved. Please, to correct this problem and avoid future problems with this tool, remove the directory from this asset and install it again.",
                    "Ok");
        }

        [MenuItem("Tools/MT Assets/Easy Minimap System/More Assets", false, 30)]
        static void MoreAssets()
        {
            Help.BrowseURL(Greetings.linkForAssetStorePage);
        }

        [MenuItem("Tools/MT Assets/Easy Minimap System/Get Support", false, 30)]
        static void GetSupport()
        {
            EditorUtility.DisplayDialog(
                "Support",
                "If you have any questions, problems or want to contact me, just contact me by email (mtassets@windsoft.xyz).",
                "Got it!");
        }
    }
}