namespace Constellation.UI {
    public static class UINodeFactory {
        public static Node<INode> GetNode (string nodeName) {
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