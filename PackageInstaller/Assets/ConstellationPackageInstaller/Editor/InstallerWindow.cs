using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

public class InstallerWindow : EditorWindow
{
    static AddRequest Request;
    static bool HasRequested;
    static bool Success;
    static bool Failure;
    [SerializeField]
    static InstallerWindow window;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Constellation Installer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        window = (InstallerWindow)EditorWindow.GetWindow(typeof(InstallerWindow));
        window.Show();
        window.maxSize = new Vector2(400, 350);
        window.minSize = new Vector2(400, 350);
    }

    void OnGUI()
    {
        var yellow = new Color(0.89f, 0.75f, 0.20f);
        var secondaryYellow = new Color(0.89f, 0.83f, 0.46f);
        var titleFontStyle = new GUIStyle();

        titleFontStyle.normal.textColor = yellow;
        titleFontStyle.fontStyle = FontStyle.Bold;
        titleFontStyle.alignment = TextAnchor.MiddleCenter;
        titleFontStyle.fontSize = 30;

        var labelFontStyle = new GUIStyle();
        labelFontStyle.normal.textColor = yellow;
        labelFontStyle.fontStyle = FontStyle.Bold;

        var background = AssetDatabase.LoadAssetAtPath("Assets/ConstellationPackageInstaller/Editor/ConstellationLogo.png", typeof(Texture2D)) as Texture2D;
        GUI.DrawTexture(new Rect(0, 0, 400, 400), background);

        if (!HasRequested)
        {
            GUILayout.Label("Constellation Setup", titleFontStyle);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);

            GUILayout.Label("Usefull links", labelFontStyle);
            GUI.color = secondaryYellow;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Web site", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://www.geek-zebra.com/home");
            }

            if (GUILayout.Button("Github", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://github.com/ConstellationLanguage/Constellation");
            }

            if (GUILayout.Button("Discord", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://discord.gg/Cx2k7We");
            }

            GUILayout.EndHorizontal();
            GUI.color = Color.white;
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
            GUILayout.Label("Requirements:", labelFontStyle);
            GUI.color = secondaryYellow;
            if (GUILayout.Button("Git", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://git-scm.com/");
            }

            GUILayout.FlexibleSpace();
            GUI.color = yellow;
            if (GUILayout.Button("Install", GUILayout.Height(EditorGUIUtility.singleLineHeight * 4)))
            {
                Request = Client.Add("https://github.com/ConstellationLanguage/Constellation.git?path=/ConstellationPackages/ConstellationPackageManager#ConstellationPackageInstaller");
                EditorApplication.update += Progress;
                HasRequested = true;
            }
            GUI.color = Color.white;
        }
        else if(Failure)
        {
            GUILayout.Label("Could not install", titleFontStyle);
            GUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
            GUILayout.Label("Setup Git before installing", labelFontStyle);
            GUI.color = secondaryYellow;

            if (GUILayout.Button("Git", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL("https://git-scm.com/");
            }

            GUILayout.FlexibleSpace();
            GUI.color = yellow;
            if (GUILayout.Button("Install", GUILayout.Height(EditorGUIUtility.singleLineHeight * 4)))
            {
                Request = Client.Add("https://github.com/ConstellationLanguage/Constellation.git?path=/ConstellationPackages/ConstellationPackageManager#ConstellationPackageInstalle");
                EditorApplication.update += Progress;
                HasRequested = true;
            }
        }
        else if (!Success)
        {
            GUILayout.Label("Processing", titleFontStyle);
        }
        else if (Success)
        {
            GUILayout.Label("Success!", titleFontStyle);
            GUILayout.FlexibleSpace();
            GUI.color = yellow;
            if (GUILayout.Button("Finish", GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                FileUtil.DeleteFileOrDirectory("yourPath/YourFileOrFolder");
                this.Close();
            }
        }
    }

    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
            {
                Success = true;
            }
            else if (Request.Status >= StatusCode.Failure)
            {
                Failure = true;
                Debug.Log(Request.Error.message);
            }

            EditorApplication.update -= Progress;
        }
    }
}
