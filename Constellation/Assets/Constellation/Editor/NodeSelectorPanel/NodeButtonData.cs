using UnityEditor;
namespace ConstellationEditor {
    public class NodeButtonData {
        public string nodeName;
        public string nodeNamespace;
        public string niceNodeName;

        public NodeButtonData (string _nodeName) {
            nodeName = nodeName.Substring (nodeName.LastIndexOf ("."));
            nodeNamespace = nodeName.Split('.')[1];
            niceNodeName = ObjectNames.NicifyVariableName (nodeName);
        }
    }
}