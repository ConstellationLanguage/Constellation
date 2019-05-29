namespace Constellation.Attributes {
    public class AttributesNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case ValueAttribute.NAME:
                    INode nodeValueAttribute = new ValueAttribute () as INode;
                    return new Node<INode> (nodeValueAttribute);
                case WordAttribute.NAME:
                    INode nodeWordAttribute = new WordAttribute() as INode;
                    return new Node<INode> (nodeWordAttribute);
                default:
                    return null;
            }
        }
    }
}