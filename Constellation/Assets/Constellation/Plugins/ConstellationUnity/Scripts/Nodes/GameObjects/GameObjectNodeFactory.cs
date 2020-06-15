namespace Constellation.GameObjects
{
    public class GameObjectsNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Transform.NAME:
                    INode nodeTransform = new Transform() as INode;
                    return new Node<INode>(nodeTransform);
                case SetActive.NAME:
                    INode nodeSetActive = new SetActive() as INode;
                    return new Node<INode>(nodeSetActive);
                case GetComponent.NAME:
                    INode nodeGetComponent = new GetComponent() as INode;
                    return new Node<INode>(nodeGetComponent);
                case Instantiate.NAME:
                    INode instantiate = new Instantiate() as INode;
                    return new Node<INode>(instantiate);
                case FindByName.NAME:
                    INode nodeFindByName = new FindByName() as INode;
                    return new Node<INode>(nodeFindByName);
                default:
                    return null;
            }
        }
    }
}