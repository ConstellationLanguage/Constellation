using System.Collections.Generic;
using UnityEngine;
namespace ConstellationEditor {
    public class NodeNamespacesData {
        public List<NodeButtonData> namespaceGroup;
        public string namespaceName;
        public List<string> nodesNiceNames;
        public List<string> nodesNames;
        public NodeNamespacesData (string _namespaceName, string[] _nodes) {
            namespaceGroup = new List<NodeButtonData> ();
            namespaceName = _namespaceName;
            nodesNames = new List<string>();
            nodesNiceNames = new List<string>();
            foreach (var node in _nodes) {
                Debug.Log(node.Split ('.') [1]);
                if (_namespaceName == node.Split ('.') [1]) {
                    var nodeButtonData = new NodeButtonData (node);
                    namespaceGroup.Add (nodeButtonData);
                    nodesNames.Add(nodeButtonData.nodeName);
                    nodesNiceNames.Add(nodeButtonData.niceNodeName);
                }
            }
            namespaceName = _namespaceName;
        }
    }
}