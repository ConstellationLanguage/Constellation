using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackageManagerWindow : EditorWindow
{

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Constellation Package Manager")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PackageManagerWindow window = (PackageManagerWindow)EditorWindow.GetWindow(typeof(PackageManagerWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Choose your version");
    }
}
