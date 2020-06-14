using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Constellation;

[System.Serializable]
public class NodeSelectorPanel
{
    Vector2 nodeSelectorScrollPos;
    public delegate void NodeAdded(string nodeName, string _namespace);
    string searchString = "";
    public List<NodeNamespacesData> NodeNamespaceData;
    private string[] namespaces;

    public NodeSelectorPanel(ConstellationScriptData[] customNodes)
    {
        
        var nodes = new List<string>(NodesFactory.GetAllNodesExcludeDiscretes());
        foreach (var customNode in customNodes)
        {
            nodes.Add(GetStaticNodeFullNameSpace() + customNode.NameSpace + "." + customNode.Name);
        }
        namespaces = NodesFactory.GetAllNamespaces(nodes.ToArray());
        NodeNamespaceData = new List<NodeNamespacesData>();

    }

    public void SetupNamespaceData(ConstellationScriptData[] customNodes)
    {
        foreach (var _namespace in namespaces)
        {
            var nodes = new List<string>(NodesFactory.GetAllNodesExcludeDiscretes());
            foreach (var customNode in customNodes)
            {
                nodes.Add(GetStaticNodeFullNameSpace() + customNode.NameSpace + "." + customNode.Name);
            }
            var nodeNamespace = new NodeNamespacesData(_namespace, nodes.ToArray());
            NodeNamespaceData.Add(nodeNamespace);
        }
    }

    private string GetStaticNodeNameSpace()
    {
        return "|" + Constellation.ConstellationTypes.StaticConstellationNode.NAME + "|";
    }

    private string GetStaticNodeFullNameSpace()
    {
        return "Constellation." + GetStaticNodeNameSpace();
    }

    private void FilterNodes(string _filer)
    {
        foreach (var nodeNameData in NodeNamespaceData)
        {
            nodeNameData.FilterNodes(_filer);
        }
    }

    public void Draw(float _width, float _height, NodeAdded OnNodeAdded, bool _allowTypesNodes)
    {
        GUILayout.BeginVertical();
        DrawSearchField();
        const int SearchFieldSize = 55; //Has to be fixed but for now it's the only way to keep the margins right
        nodeSelectorScrollPos = EditorGUILayout.BeginScrollView(nodeSelectorScrollPos, GUILayout.Width(_width), GUILayout.Height(_height - SearchFieldSize));
        foreach (NodeNamespacesData nodeNamespace in NodeNamespaceData)
        {
            if(!_allowTypesNodes && nodeNamespace.namespaceName == Constellation.ConstellationTypes.NameSpace.NAME) 
            {
                continue;
            }
            var displayedNamespace = nodeNamespace.namespaceName.Replace(GetStaticNodeNameSpace(), "");
            GUILayout.Label(displayedNamespace, GUI.skin.GetStyle("OL Title"), GUILayout.Width(_width - 20));
            var selGridInt = GUILayout.SelectionGrid(-1, nodeNamespace.GetNiceNames(), 1 + (int)Mathf.Floor(_width / 255));
            if (selGridInt >= 0)
            {
                OnNodeAdded(nodeNamespace.GetNames()[selGridInt], nodeNamespace.namespaceName);
            }
        }
        //GUILayout.Space(20);
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void ClearSerachField()
    {
        searchString = "";
        FilterNodes(searchString);
    }

    private void DrawSearchField()
    {
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        var newSearchString = searchString;
        newSearchString = GUILayout.TextField(newSearchString, GUI.skin.FindStyle("ToolbarSeachTextField"));

        if (newSearchString != searchString)
        {
            searchString = newSearchString;
            FilterNodes(searchString);
        }

        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            ClearSerachField();
            //GUI.FocusControl(null);
        }

        GUILayout.EndHorizontal();
    }
}
