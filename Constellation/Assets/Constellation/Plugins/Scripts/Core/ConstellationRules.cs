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
                        nodeData.Outputs[genericOutputsID[j]].Type = UndefinedName;
                    }
                }
            }

            nodeData = constellationScript.AddNode(nodeData);
            nodeData.XPosition = 0;
            nodeData.YPosition = 0;

            return nodeData;
        }

        public static void UpdateGenericNodeByLinkGUID(ConstellationScript constellationScript, NodesFactory nodesFactory, string guid)
        {
            var linkedinputID = 0;
            var linkedOutputID = 0;
            var connectedNodes = constellationScript.GetNodesWithLinkGUID(guid, out linkedinputID, out linkedOutputID);
            var outputNode = connectedNodes[0];
            var inputNode = connectedNodes[1];
            var inputNodeScript = nodesFactory.GetNode(inputNode).NodeType as IGenericNode;
            if (inputNodeScript != null && inputNodeScript.IsGenericInput(linkedinputID))
            {
                var inputsID = inputNodeScript.GetGenericInputByLinkedOutput(linkedOutputID);

                for (var k = 0; k < inputNode.GetInputs().Length; k++)
                {
                    for (var l = 0; l < inputsID.Length; l++)
                    {
                        if (k == inputsID[l])
                        {
                            inputNode.Inputs[k].Type = outputNode.Outputs[linkedOutputID].Type;
                        }
                    }
                }
                if (inputNodeScript.IsGenericInput(linkedinputID))
                {
                    var outputID = inputNodeScript.GetGenericOutputByLinkedInput(linkedinputID);
                    for (var k = 0; k < inputNode.GetOutputs().Length; k++)
                    {
                        for (var l = 0; l < outputID.Length; l++)
                        {
                            if (k == outputID[l])
                            {
                                inputNode.Outputs[k].Type = outputNode.Outputs[linkedOutputID].Type;
                            }
                        }
                    }
                }
            }
        }

        public static void RemoveNode(NodeData node, ConstellationScript constellationScript)
        {
            constellationScript.RemoveNode(node);
        }
    }
}
