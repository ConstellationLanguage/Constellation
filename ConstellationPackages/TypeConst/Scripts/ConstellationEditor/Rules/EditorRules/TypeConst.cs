namespace Constellation
{
    public class TypeConst : IConstellationEditorRule
    {
        public NodeData NodeAdded(NodeData nodeData, Node<INode> newNode, ConstellationScriptData constellationScript)
        {
            var genericNode = newNode.NodeType as IGenericNode;
            if (genericNode as IGenericNode != null)
            {
                for (var i = 0; i < newNode.Inputs.Count; i++)
                {
                    var genericOutputsID = genericNode.GetGenericOutputByLinkedInput(i);
                    for (var j = 0; j < genericOutputsID.Length; j++)
                    {
                        nodeData.Outputs[genericOutputsID[j]].Type = ConstellationEditorRules.UNDEFINED;
                    }
                }
            }

            return nodeData;
        }

        public void LinkAdded(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid, IConstellationFileParser constellationParser)
        {
            var linkedinputID = 0;
            var linkedOutputID = 0;
            var connectedNodes = constellationScript.GetNodesWithLinkGUID(guid, out linkedinputID, out linkedOutputID);
            var outputNode = connectedNodes[0];
            var inputNode = connectedNodes[1];
            var inputNodeType = nodesFactory.GetNode(inputNode, constellationParser).NodeType;
            var outputNodeType = nodesFactory.GetNode(outputNode, constellationParser).NodeType;
            var inputGenericNodeScript = inputNodeType as IGenericNode;
            var mirrorInputNodeScript = inputNodeType as IMirrorNode;
            var mirrorOutputNodeScript = outputNodeType as IMirrorNode;

            if (mirrorInputNodeScript != null)
            {
                for (var k = 0; k < inputNode.GetInputs().Length; k++)
                {
                    inputNode.Inputs[k].Type = outputNode.Outputs[linkedOutputID].Type;
                }
            }
            else if (mirrorOutputNodeScript != null)
            {
                for (var k = 0; k < outputNode.GetOutputs().Length; k++)
                {
                    outputNode.Outputs[k].Type = inputNode.Inputs[linkedinputID].Type;
                }
            }
            else
            {
                if (inputGenericNodeScript != null && inputGenericNodeScript.IsGenericInput(linkedinputID))
                {
                    var inputsID = linkedinputID;

                    for (var k = 0; k < inputNode.GetInputs().Length; k++)
                    {
                        if (k == linkedinputID)
                        {
                            inputNode.Inputs[k].Type = outputNode.Outputs[linkedOutputID].Type;
                            break;
                        }
                    }
                    if (inputGenericNodeScript.IsGenericInput(linkedinputID))
                    {
                        var outputID = inputGenericNodeScript.GetGenericOutputByLinkedInput(linkedinputID);
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
        }

        public bool IsLinkValid(InputData _input, OutputData _output)
        {
            return (_input != null && _output != null) && (_input.Type == _output.Type || (_input.Type == ConstellationEditorRules.ANY && _output.Type != ConstellationEditorRules.GENERIC) || (_input.Type != ConstellationEditorRules.GENERIC && _output.Type == ConstellationEditorRules.ANY) || (_input.Type != ConstellationEditorRules.ANY && _output.Type == ConstellationEditorRules.GENERIC) || (_input.Type == ConstellationEditorRules.GENERIC && _output.Type != ConstellationEditorRules.ANY));
        }

        public bool CanRemoveNode(NodeData node, ConstellationScriptData constellationScript)
        {
            return true;
        }
    }
}