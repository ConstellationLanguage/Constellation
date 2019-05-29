namespace Constellation.Tags {
    public class TagsNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Tutorial.NAME:
                    INode nodeTutorial = new Tutorial () as INode;
                    return new Node<INode> (nodeTutorial);
                case Nestable.NAME:
                    INode nodeNestable = new Nestable() as INode;
                    return new Node<INode>(nodeNestable);
                default:
                    return null;
            }
        }
    }
}