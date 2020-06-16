namespace Constellation.Unity {
    public class UnityNodeFactory : INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case DeltaTime.NAME:
                    INode nodeDeltaTime = new DeltaTime () as INode;
                    return new Node<INode> (nodeDeltaTime);
                case ObjectParameter.NAME:
                    INode nodeObjectAttribute = new ObjectParameter () as INode;
                    return new Node<INode> (nodeObjectAttribute);
                case Update.NAME:
                    INode nodeUpdate = new Update () as INode;
                    return new Node<INode> (nodeUpdate);
                case LateUpdate.NAME:
                    INode nodeLateUpdate = new LateUpdate () as INode;
                    return new Node<INode> (nodeLateUpdate);
                case PlayerPreferences.NAME:
                    INode nodePlayerPref = new PlayerPreferences () as INode;
                    return new Node<INode> (nodePlayerPref);
                case LoadScene.NAME:
                    INode nodeLoadScene = new LoadScene () as INode;
                    return new Node<INode> (nodeLoadScene);
                case Quit.NAME:
                    INode quit = new Quit() as INode;
                    return new Node<INode> (quit);
                case AddConstellationBehaviourFromJSON.NAME:
                    INode addConstellationBehaviourFromJSON = new AddConstellationBehaviourFromJSON() as INode;
                    return new Node<INode>(addConstellationBehaviourFromJSON);
                default:
                    return null;
            }
        }
    }
}