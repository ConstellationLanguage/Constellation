namespace Constellation.Parameters {
    public class ParametersNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case ValueParameter.NAME:
                    INode nodeValueAttribute = new ValueParameter () as INode;
                    return new Node<INode> (nodeValueAttribute);
                case WordParameter.NAME:
                    INode nodeWordAttribute = new WordParameter() as INode;
                    return new Node<INode> (nodeWordAttribute);
                default:
                    return null;
            }
        }
    }
}