namespace Constellation.UI {
    public class UINodeFactory: INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Text.NAME:
                    INode nodeText = new Text () as INode;
                    return new Node<INode> (nodeText);
                default:
                    return null;
            }
        }
    }
}