
namespace Constellation.Components {
    public class UnityComponentsNodeFactory: INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case AnimatorComponent.NAME:
                    var nodeAnimator = new AnimatorComponent () as INode;
                    return new Node<INode> (nodeAnimator);
                default:
                    return null;
            }
        }
    }
}