namespace Constellation.Tags {
    public class TagsNodeFactory: INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Tutorial.NAME:
                    INode nodeTutorial = new Tutorial () as INode;
                    return new Node<INode> (nodeTutorial);
                case Component.NAME:
                    INode nodeComponent = new Component() as INode;
                    return new Node<INode> (nodeComponent);
                default:
                    return null;
            }
        }
    }
}