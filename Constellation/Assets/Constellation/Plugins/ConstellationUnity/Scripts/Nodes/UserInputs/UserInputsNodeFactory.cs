
namespace Constellation.UserInputs
{
    public class UserInputsNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case KeyDown.NAME:
                    INode nodeKeyDown = new KeyDown() as INode;
                    return new Node<INode>(nodeKeyDown);
                case Key.NAME:
                    INode nodeKey = new Key() as INode;
                    return new Node<INode>(nodeKey);
                case MouseButtonDown.NAME:
                    INode nodeMouseButtonDown = new MouseButtonDown() as INode;
                    return new Node<INode>(nodeMouseButtonDown);
                case MousePosition.NAME:
                    INode nodeMousePosition = new MousePosition() as INode;
                    return new Node<INode>(nodeMousePosition);
                default:
                    return null;
            }
        }
    }
}