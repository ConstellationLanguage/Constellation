namespace Constellation.ConstellationTypes {
    public class ConstellationTypesFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Tutorial.NAME:
                    INode nodeTutorial = new Tutorial () as INode;
                    return new Node<INode> (nodeTutorial);
                case ConstellationNode.NAME:
                    INode nodeNestable = new ConstellationNode() as INode;
                    return new Node<INode>(nodeNestable);
                default:
                    return null;
            }
        }
    }
}