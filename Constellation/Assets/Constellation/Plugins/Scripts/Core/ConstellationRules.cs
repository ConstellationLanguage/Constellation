namespace Constellation
{
    public static class ConstellationRules
    {
        const string UndefinedName = "Undefined";

        public static NodeData AddNode(NodesFactory nodesFactory, string nodeName, string nodeNamespace, ConstellationScript constellationScript)
        {
            var newNode = nodesFactory.GetNode(nodeName, nodeNamespace);
            var nodeData = new NodeData(newNode);
            var genericNode = newNode.NodeType as IGenericNode;
            if (genericNode as IGenericNode != null)
            {
                for (var i = 0; i < newNode.Inputs.Count; i++)
                {
                    var genericOutputsID = genericNode.GetGenericOutputByLinkedInput(i);
                    for (var j = 0; j < genericOutputsID.Length; j++)
                    {
                        nodeData.Outputs[genericOutputsID[j]].Type = "Undefined";
                    }
                }
            }

            nodeData = constellationScript.AddNode(nodeData);
            nodeData.XPosition = 0;
            nodeData.YPosition = 0;

            return nodeData;
        }

        public static void RemoveNode(NodeData node, ConstellationScript constellationScript)
        {
            constellationScript.RemoveNode(node);
        }
    }
}
