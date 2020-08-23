namespace Constellation
{
    public interface IConstellationEditorRule
    {
        NodeData NodeAdded(NodeData nodeData, Node<INode> newNode, ConstellationScriptData constellationScript);
        bool IsLinkValid(InputData _input, OutputData _output);
        bool CanRemoveNode(NodeData node, ConstellationScriptData constellationScript);
        void LinkAdded(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid, IConstellationFileParser constellationParser);
    }
}
