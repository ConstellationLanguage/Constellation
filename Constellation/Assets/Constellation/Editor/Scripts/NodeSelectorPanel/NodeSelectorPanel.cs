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
                NodeNamespaceData.Add (nodeNamespace);
            }
        }

        private void FilterNodes (string _filer) {
            foreach (var nodeNameData in NodeNamespaceData) {
                nodeNameData.FilterNodes (_filer);
            }
        }

        public void Draw (float _width, float _height) {
            GUILayout.BeginVertical ();
            DrawSearchField ();
            nodeSelectorScrollPos = EditorGUILayout.BeginScrollView (nodeSelectorScrollPos, GUILayout.Width (_width), GUILayout.Height (_height));
            foreach (NodeNamespacesData nodeNamespace in NodeNamespaceData) {
                GUILayout.Label (nodeNamespace.namespaceName, GUI.skin.GetStyle ("OL Title"), GUILayout.Width(_width - 20));
                var selGridInt = GUILayout.SelectionGrid (-1, nodeNamespace.GetNiceNames (), 1 + (int)Mathf.Floor(_width / 255));
                if (selGridInt >= 0) {
                    OnNodeAdded (nodeNamespace.GetNames () [selGridInt], nodeNamespace.namespaceName);
                }
            }
            EditorGUILayout.EndScrollView ();
            GUILayout.EndVertical ();
        }

        private void ClearSerachField () {
            searchString = "";
            FilterNodes (searchString);
        }

        private void DrawSearchField () {
            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.fieldWidth = 0;
            GUILayout.BeginHorizontal (GUI.skin.FindStyle ("Toolbar"));
            var newSearchString = searchString;
            newSearchString = GUILayout.TextField (newSearchString, GUI.skin.FindStyle ("ToolbarSeachTextField"));

            if (newSearchString != searchString) {
                searchString = newSearchString;
                FilterNodes (searchString);
            }

            if (GUILayout.Button ("", GUI.skin.FindStyle ("ToolbarSeachCancelButton"))) {
                ClearSerachField ();
                GUI.FocusControl (null);
            }

            GUILayout.EndHorizontal ();
        }
    }
}