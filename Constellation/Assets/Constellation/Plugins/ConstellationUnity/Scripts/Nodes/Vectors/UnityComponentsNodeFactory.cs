
namespace Constellation.Vectors {
    public class UnityVectorsNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Forward.NAME:
                    var nodeForward = new Forward () as INode;
                    return new Node<INode> (nodeForward);
                default:
                    return null;
            }
        }
    }
}