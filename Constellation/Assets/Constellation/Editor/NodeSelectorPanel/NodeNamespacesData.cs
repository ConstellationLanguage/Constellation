using System.Collections.Generic;
using UnityEngine;
namespace ConstellationEditor {
    public class NodeNamespacesData {
        public List<NodeButtonData> namespaceGroup;
        public string namespaceName;
        List<string> nodesNiceNames = new List<string> ();
        List<string> nodesNames = new List<string> ();
        public NodeNamespacesData (string _namespaceName, string[] _nodes) {
            namespaceGroup = new List<NodeButtonData> ();
            namespaceName = _namespaceName;
            foreach (var node in _nodes) {
                Debug.Log (node.Split ('.') [1]);
                if (_namespaceName == node.Split ('.') [1]) {
                    var nodeButtonData = new NodeButtonData (node);
                    namespaceGroup.Add (nodeButtonData);
                }
            }
            FilterNodes("");
            RefreshNamesList();
            namespaceName = _namespaceName;
        }

        public void FilterNodes (string _filterName) {
            foreach (var group in namespaceGroup) {
                if (group.niceNodeName.Contains (_filterName) || _filterName == "" || _filterName == null)
                    group.Display ();
                else
                    group.Hide ();
            }
            RefreshNamesList();
        }

        private void RefreshNamesList () {
            nodesNames = new List<string> ();
            nodesNiceNames = new List<string> ();

            foreach (var group in namespaceGroup) {
                if (group.display){
                    nodesNiceNames.Add (group.niceNodeName);
                    nodesNames.Add (group.nodeName);
                }
            }
        }

        public string[] GetNiceNames () {

            return nodesNiceNames.ToArray ();
        }

        public string[] GetNames () {

            return nodesNames.ToArray ();
        }
    }
}