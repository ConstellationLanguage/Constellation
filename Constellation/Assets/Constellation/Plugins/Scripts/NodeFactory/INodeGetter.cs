namespace Constellation {
    public interface INodeGetter {
        Node<INode> GetNode (string nodeName);
        string GetNameSpace();
    }
}