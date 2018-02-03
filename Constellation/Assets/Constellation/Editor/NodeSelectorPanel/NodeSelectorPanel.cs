using System;
using System.Collections.Generic;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeSelectorPanel {
        Vector2 nodeSelectorScrollPos;

        public delegate void NodeAdded (string nodeName, string _namespace);
        NodeAdded OnNodeAdded;
        string searchString = "";
        private List<NodeNamespacesData> NodeNamespaceData;
        private string[] namespaces;

        public NodeSelectorPanel (NodeAdded _onNodeAdded) {
            OnNodeAdded = null;
            OnNodeAdded += _onNodeAdded;
            var nodes = NodesFactory.GetAllNodes ();
            namespaces = NodesFactory.GetAllNamespaces (nodes);
            NodeNamespaceData = new List<NodeNamespacesData> ();
            foreach (var _namespace in namespaces) {
                var nodeNamespace = new NodeNamespacesData (_namespace, nodes);
                NodeNamespaceData.Add(nodeNamespace);
            }
        }

        public void Draw (float _width, float _height) {
            GUILayout.BeginVertical ();
            DrawSearchField ();
            nodeSelectorScrollPos = EditorGUILayout.BeginScrollView (nodeSelectorScrollPos, GUILayout.Width (_width), GUILayout.Height (_height));
            foreach (NodeNamespacesData nodeNamespace in NodeNamespaceData) {
                GUILayout.Label (nodeNamespace.namespaceName, GUI.skin.GetStyle ("OL Title"));
               var selGridInt = GUILayout.SelectionGrid (-1, nodeNamespace.nodesNiceNames.ToArray(), 2);
                if (selGridInt >= 0) {
                    ClearSerachField ();
                    OnNodeAdded (nodeNamespace.nodesNames[selGridInt], nodeNamespace.namespaceName);
                }
            }
            EditorGUILayout.EndScrollView ();
            GUILayout.EndVertical ();
        }

        private void ClearSerachField () {
            searchString = "";
        }

        private void DrawSearchField () {
            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.fieldWidth = 0;
            GUILayout.BeginHorizontal (GUI.skin.FindStyle ("Toolbar"));
            searchString = GUILayout.TextField (searchString, GUI.skin.FindStyle ("ToolbarSeachTextField"));
            if (GUILayout.Button ("", GUI.skin.FindStyle ("ToolbarSeachCancelButton"))) {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl (null);
            }
            GUILayout.EndHorizontal ();
        }
    }
}