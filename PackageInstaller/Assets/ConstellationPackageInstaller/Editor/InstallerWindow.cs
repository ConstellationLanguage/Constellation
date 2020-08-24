using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InstallerWindow : EditorWindow
{

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Constellation Installer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        InstallerWindow window = (InstallerWindow)EditorWindow.GetWindow(typeof(InstallerWindow));
        window.Show();
        window.maxSize = new Vector2(200, 200);
        window.minSize = new Vector2(200, 200);
    }

    void OnGUI()
    {
        GUILayout.Label("This utility will install \n Constellation package manager.");
        if (GUILayout.Button("Web site"))
        {
            Application.OpenURL("http://constellationeditor.com/");
        }
        
        if (GUILayout.Button("Github"))
        {
            Application.OpenURL("https://github.com/ConstellationLanguage/Constellation");
        }

        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        GUILayout.Label("Requirements:");
        if (GUILayout.Button("Git"))
        {
            Application.OpenURL("https://git-scm.com/");
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Install"))
        {

        }
    }
}
