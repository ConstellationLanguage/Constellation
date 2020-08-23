using System;
using System.Collections.Generic;

[System.Serializable]
public class NodeNamespacesData
{
    public List<NodeButtonData> namespaceGroup;
    public string namespaceName;
    public string[] nodes;
    public List<string> nodesNiceNames = new List<string>();
    public List<string> nodesNames = new List<string>();

    public NodeNamespacesData(string _namespaceName, string[] _nodes)
    {
        nodes = _nodes;
        namespaceGroup = new List<NodeButtonData>();
        namespaceName = _namespaceName;
        foreach (var node in nodes)
        {
            if (_namespaceName == node.Split('.')[1])
            {
                var nodeButtonData = new NodeButtonData(node);
                namespaceGroup.Add(nodeButtonData);
            }
        }
        FilterNodes("");
        RefreshNamesList();
        namespaceName = _namespaceName;
    }

    //Cannot be setted in the constructor due to serialization limitation

    public void FilterNodes(string _filterName)
    {
        foreach (var group in namespaceGroup)
        {
            if (group.niceNodeName.IndexOf(_filterName, StringComparison.CurrentCultureIgnoreCase) > 0 ||
                group.nodeName.IndexOf(_filterName, StringComparison.CurrentCultureIgnoreCase) > 0 ||
                group.nodeFullName.IndexOf(_filterName, StringComparison.CurrentCultureIgnoreCase) > 0 ||
                _filterName == "" ||
                _filterName == null)
                group.Display();
            else
                group.Hide();
        }
        RefreshNamesList();
    }

    private void RefreshNamesList()
    {
        nodesNames = new List<string>();
        nodesNiceNames = new List<string>();

        foreach (var group in namespaceGroup)
        {
            if (group.display)
            {
                nodesNiceNames.Add(group.niceNodeName);
                nodesNames.Add(group.nodeName);
            }
        }
    }

    public string[] GetNiceNames()
    {

        return nodesNiceNames.ToArray();
    }

    public string[] GetNames()
    {

        return nodesNames.ToArray();
    }
}