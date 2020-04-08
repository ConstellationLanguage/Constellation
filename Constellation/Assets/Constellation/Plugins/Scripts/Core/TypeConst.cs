namespace Constellation
{
    public static class TypeConst
    {
        public delegate void LinkValid();
        public delegate void LinkAdded(string linkGuid);
        public const string UNDEFINED = "Undefined";
        public const string ANY = "Any";
        public const string GENERIC = "Generic";
        public const string VAR = "Var";

        public static NodeData AddNode(NodesFactory nodesFactory, string nodeName, string nodeNamespace, ConstellationScriptData constellationScript)
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
                        nodeData.Outputs[genericOutputsID[j]].Type = UNDEFINED;
                    }
                }
            }

            nodeData = constellationScript.AddNode(nodeData);
            nodeData.XPosition = 0;
            nodeData.YPosition = 0;

            return nodeData;
        }

        public static void UpdateGenericNodeByLinkGUID(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid)
        {
            var linkedinputID = 0;
            var linkedOutputID = 0;
            var connectedNodes = constellationScript.GetNodesWithLinkGUID(guid, out linkedinputID, out linkedOutputID);
            var outputNode = connectedNodes[0];
            var inputNode = connectedNodes[1];
            var inputNodeType = nodesFactory.GetNode(inputNode).NodeType;
            var outputNodeType = nodesFactory.GetNode(outputNode).NodeType;
            var inputGenericNodeScript = inputNodeType as IGenericNode;
            var mirrorInputNodeScript = inputNodeType as IMirrorNode;
            var mirrorOutputNodeScript = outputNodeType as IMirrorNode;

            if (mirrorInputNodeScript != null)
            {
                for (var k = 0; k < inputNode.GetInputs().Length; k++)
                {
                    inputNode.Inputs[k].Type = outputNode.Outputs[linkedOutputID].Type;
                }
            } else if(mirrorOutputNodeScript != null)
            {
                for (var k = 0; k < outputNode.GetOutputs().Length; k++)
                {
                    outputNode.Outputs[k].Type = inputNode.Inputs[linkedinputID].Type;
                }
            } else
            {
                if (inputGenericNodeScript != null && inputGenericNodeScript.IsGenericInput(linkedinputID))
                {
                    var inputsID = inputGenericNodeScript.GetGenericInputByLinkedOutput(linkedOutputID);

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

        public static bool CreateLink(InputData _input, OutputData _output, ConstellationScriptData constellationScript, LinkValid linkIsValid, LinkAdded linkCreated)
        {
            if (_output != null && _output.Type == UNDEFINED && _input != null && _input.Type != UNDEFINED)
                return false;

            //if ()
            if (IsTypeValid(_input, _output))
            {
                var newLink = new LinkData(_input, _output);
                if (TypeConst.IsLinkValid(newLink, constellationScript))
                {
                    linkIsValid();
                    constellationScript.AddLink(newLink);
                    linkCreated(newLink.GUID);
                    return true;
                }
            }
            return false;
        }

        public static bool IsTypeValid(InputData _input, OutputData _output)
        {
            return (_input != null && _output != null) && (_input.Type == _output.Type || (_input.Type == ANY && _output.Type != GENERIC) || (_input.Type != GENERIC && _output.Type == ANY) || (_input.Type != ANY && _output.Type == GENERIC) || (_input.Type == GENERIC && _output.Type != ANY));
        }

        public static bool IsLinkValid(LinkData _link, ConstellationScriptData _constellationScriptData)
        {
            foreach (LinkData link in _constellationScriptData.Links)
            {
                if (_link.Input.Guid == link.Input.Guid && _link.Output.Guid == link.Output.Guid)
                {
                    return false;
                }
            }
            return true;
        }

        public static void RemoveNode(NodeData node, ConstellationScriptData constellationScript)
        {
            constellationScript.RemoveNode(node.Guid);
        }
    }
}
