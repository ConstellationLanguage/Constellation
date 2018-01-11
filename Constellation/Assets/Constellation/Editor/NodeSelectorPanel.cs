using UnityEngine;
using Constellation;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace ConstellationEditor
{
    public class NodeSelectorPanel
    {
        Vector2 nodeSelectorScrollPos;
        private string[] nodes;
        private string[] namespaces;
        public delegate void NodeAdded(string nodeName, string _namespace);
        NodeAdded OnNodeAdded;
        string searchString = "";

        public NodeSelectorPanel(NodeAdded _onNodeAdded)
        {
            OnNodeAdded = null;
            OnNodeAdded += _onNodeAdded;
            nodes = NodesFactory.GetAllNodes();
            namespaces = NodesFactory.GetAllNamespaces(nodes);
        }

        public void Draw(float _width, float _height)
        {
            GUILayout.BeginVertical();
            DrawSearchField();
            nodeSelectorScrollPos = EditorGUILayout.BeginScrollView(nodeSelectorScrollPos, GUILayout.Width(_width), GUILayout.Height(_height));
            foreach (string nodeNamespace in namespaces)
            {
                GUILayout.Label(nodeNamespace, GUI.skin.GetStyle("OL Title"));
                List<string> nodesName = new List<string>();
                List<string> nodesNiceName = new List<string>();
                foreach (string node in nodes)
                {

                    if ((node.IndexOf(searchString, 0, StringComparison.CurrentCultureIgnoreCase) != -1 || searchString == "") && node.IndexOf(nodeNamespace, 0, StringComparison.CurrentCulture) != -1)
                    {
                        var nodeTitle = node.Substring(node.LastIndexOf(".") + 1);
                        nodesName.Add(nodeTitle);
                        nodesNiceName.Add(ObjectNames.NicifyVariableName(nodeTitle));    
                    }
                }
                var selGridInt = GUILayout.SelectionGrid(-1, nodesNiceName.ToArray(), 2);
                if (selGridInt >= 0){
                    ClearSerachField();
                    OnNodeAdded(nodesName[selGridInt], nodeNamespace );
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void ClearSerachField()
        {
            searchString = "";
        }

        private void DrawSearchField()
        {
            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.fieldWidth = 0;
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
        }
    }
}