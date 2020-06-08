namespace Constellation
{
    public interface IConstellationRule
    {
        NodeData NodeAdded(NodeData nodeData, Node<INode> newNode, ConstellationScriptData constellationScript);
        bool IsTypeValid(InputData _input, OutputData _output);
        bool IsLinkValid(LinkData _link, ConstellationScriptData _constellationScriptData);
        //bool LinkAdded(InputData _input, OutputData _output, ConstellationScriptData constellationScript, ConstellationRules.LinkValid linkIsValid, ConstellationRules.LinkAdded linkCreated);
        void RemoveNode(NodeData node, ConstellationScriptData constellationScript);
        void UpdateGenericNodeByLinkGUID(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid);
    }
}
