namespace Constellation
{
    public interface IConstellationEditorRule
    {
        NodeData NodeAdded(NodeData nodeData, Node<INode> newNode, ConstellationScriptData constellationScript);
        bool IsTypeValid(InputData _input, OutputData _output);
        bool IsLinkValid(LinkData _link, ConstellationScriptData _constellationScriptData);
        void RemoveNode(NodeData node, ConstellationScriptData constellationScript);
        void UpdateGenericNodeByLinkGUID(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid);
    }
}
