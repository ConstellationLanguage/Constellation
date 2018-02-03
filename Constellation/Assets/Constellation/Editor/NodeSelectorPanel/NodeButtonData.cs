using UnityEditor;
namespace ConstellationEditor {
    public class NodeButtonData {
        public string nodeFullName;
        public string nodeName;
        public string nodeNamespace;
        public string niceNodeName;
        public bool display = true;

        public NodeButtonData (string _nodeName) {
            display = true;
            nodeFullName = _nodeName;
            nodeName = nodeFullName.Split('.')[2];
            nodeNamespace = nodeFullName.Split('.')[1];
            niceNodeName = ObjectNames.NicifyVariableName (nodeFullName.Split('.')[2]);
        }

        public void Display()
        {
            display = true;
        }

        public void Hide()
        {
            display = false;
        }
    }
}