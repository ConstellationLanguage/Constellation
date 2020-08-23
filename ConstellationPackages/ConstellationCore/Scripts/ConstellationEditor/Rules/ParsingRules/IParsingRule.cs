namespace Constellation
{
        public interface IParsingRule
        {
            bool isNodeValid(NodeData nodeData, Node<INode> node, NodesFactory nodesFactory);
        }
}
