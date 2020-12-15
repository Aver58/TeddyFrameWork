using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames
{
    public static class Heureka_WindowStyler
    {
        public static readonly Color clr_Pink = new Color((226f / 256f), (32f / 256f), (140f / 256f), 1);
        public static readonly Color clr_Dark = new Color((48f / 256f), (41f / 256f), (47f / 256f), 1);
        public static readonly Color clr_dBlue = new Color((47f / 256f), (102f / 256f), (144f / 256f), 1);
        public static readonly Color clr_lBlue = new Color((58f / 256f), (124f / 256f), (165f / 256f), 1);
        public static readonly Color clr_White = new Color((217f / 256f), (220f / 256f), (214f / 256f), 1);

        public static void DrawGlobalHeader(EditorWindow window, Texture icon, Color color, string label, string version = "")
        {
            if (window == null)
                return;

            EditorGUI.DrawRect(new Rect(0, 0, window.position.width, 24), color);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            //GUILayout.Label(icon, GUILayout.Width(22), GUILayout.Height(22) /*GUILayout.MaxHeight(22)*/);
            if (Heureka_EditorData.Instance.HeadlineStyle != null)
                GUILayout.Label(label, Heureka_EditorData.Instance.HeadlineStyle);

            if (version!="")
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                GUILayout.Label(version, EditorStyles.whiteLabel);
                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawCenteredImage(EditorWindow window, Texture image)
        {
            if (window == null)
                return;

            GUI.Box(new Rect((window.position.width * .5f) - (image.width * .5f), (window.position.height * .5f) - (image.height * .5f), image.width, image.height), image, GUIStyle.none);
        }

        public static void DrawCenteredMessage(EditorWindow window, Texture icon, float msgWidth, float msgHeight, string messsage)
        {
            if (window == null)
                return;

            Vector2 outerBoxSize = new Vector2(msgWidth, msgHeight);
            float frameWidth = 5;
            Vector2 innerBoxSize = new Vector2(outerBoxSize.x - frameWidth * 2, outerBoxSize.y - frameWidth * 2);

            Vector2 rectStartPos = new Vector2((window.position.width * .5f) - (outerBoxSize.x * .5f), (window.position.height * .5f) - (outerBoxSize.y * .5f) + (icon.height * .5f));

            EditorGUI.DrawRect(new Rect(rectStartPos.x, rectStartPos.y, outerBoxSize.x, outerBoxSize.y), Heureka_WindowStyler.clr_White);
            EditorGUI.DrawRect(new Rect(rectStartPos.x + frameWidth, rectStartPos.y + frameWidth, innerBoxSize.x, innerBoxSize.y), Heureka_WindowStyler.clr_dBlue);

            float bounds = 20;
            Vector2 logoStartPos = rectStartPos + new Vector2(bounds, bounds);
            GUI.Box(new Rect(logoStartPos.x, logoStartPos.y, icon.width, icon.height), icon, GUIStyle.none);

            Vector2 labelStartPos = logoStartPos + new Vector2(icon.width + frameWidth * 2, 0);
            float textWidth = innerBoxSize.x - icon.width - (bounds * 2);
            float textHeight = innerBoxSize.y - icon.height - (bounds * 2);
            GUI.Label(new Rect(labelStartPos.x, labelStartPos.y, textWidth, textHeight), messsage, Heureka_EditorData.Instance.HeadlineStyle);
        }
    }
}