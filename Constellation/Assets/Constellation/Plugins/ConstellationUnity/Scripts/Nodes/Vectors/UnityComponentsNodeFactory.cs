
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
                case Vec3.NAME:
                    INode nodeVec3 = new Vec3() as INode;
                    return new Node<INode>(nodeVec3);
                case SplitVec3.NAME:
                    INode nodeSplitVec3 = new SplitVec3() as INode;
                    return new Node<INode>(nodeSplitVec3);
                case LookAtPosition.NAME:
                    INode nodeLookAt = new LookAtPosition() as INode;
                    return new Node<INode>(nodeLookAt);
                case ScreenToWorld.NAME:
                    INode nodeScreenToWorld = new ScreenToWorld() as INode;
                    return new Node<INode>(nodeScreenToWorld);
                default:
                    return null;
            }
        }
    }
}