using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConstellationEditorWindowV2 : EditorWindow
{
    public NodeWindow NodeWindow;
    public NodeSelectorPanel nodeSelector;
    private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
    
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConstellationEditorWindowV2 window = (ConstellationEditorWindowV2)EditorWindow.GetWindow(typeof(ConstellationEditorWindowV2));
        window.Show();
    }

    public void Awake()
    {
        NodeWindow = new NodeWindow(editorPath);
        nodeSelector = new NodeSelectorPanel(/*, scriptDataService.GetAllCustomNodesNames()*/);
    }

    void NodeAdded(string nodeName, string _namespace)
    {
        NodeWindow.AddNode(nodeName);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        NodeWindow.UpdateGUI(RequestRepaint, position.width, position.height);
        nodeSelector.Draw(300, position.height, NodeAdded);
        EditorGUILayout.EndHorizontal();

    }

    void RequestRepaint()
    {
        Repaint();
    }
}
