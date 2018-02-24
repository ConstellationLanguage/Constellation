namespace Constellation.UI {
    public class UINodeFactory: INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Text.NAME:
                    INode nodeText = new Text () as INode;
                    return new Node<INode> (nodeText);
                case Button.NAME:
                    INode nodeButton = new Button() as INode;
                    return new Node<INode> (nodeButton);
                case Image.NAME:
                    INode nodeImage = new Image() as INode;
                    return new Node<INode> (nodeImage);
                default:
                    return null;
            }
        }
    }
}