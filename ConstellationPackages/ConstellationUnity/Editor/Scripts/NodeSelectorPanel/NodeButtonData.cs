using UnityEditor;
[System.Serializable]
public class NodeButtonData
{
    public string nodeFullName;
    public string nodeName;
    public string nodeNamespace;
    public string niceNodeName;
    public bool display = true;
    const int maxCharacters = 18;
    public NodeButtonData(string _nodeName)
    {
        display = true;
        nodeFullName = _nodeName;
        nodeName = nodeFullName.Split('.')[2];
        niceNodeName = ObjectNames.NicifyVariableName(nodeFullName.Split('.')[2]);
        nodeNamespace = nodeFullName.Split('.')[1];
        if (niceNodeName.Length > maxCharacters)
        {
            niceNodeName = niceNodeName.Substring(0, maxCharacters - 3);
            niceNodeName = niceNodeName + "...";
        }
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