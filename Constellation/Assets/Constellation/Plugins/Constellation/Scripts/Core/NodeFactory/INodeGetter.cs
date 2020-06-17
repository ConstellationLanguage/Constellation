namespace Constellation {
    public interface INodeGetter {
        Node<INode> GetNode (string nodeName, IConstellationFileParser constellationFileParser);
        string GetNameSpace();
    }
}