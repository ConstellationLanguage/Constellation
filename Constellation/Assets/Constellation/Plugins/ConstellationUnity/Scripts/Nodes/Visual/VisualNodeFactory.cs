namespace Constellation.Visual {
    public class VisualNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Color.NAME:
                    INode nodeColor = new Color() as INode;
                    return new Node<INode>(nodeColor);
                case MaterialColor.NAME:
                    INode materialColor = new MaterialColor() as INode;
                    return new Node<INode>(materialColor);
                case ParticlesEmission.NAME:
                    INode particleEmission = new ParticlesEmission() as INode;
                    return new Node<INode>(particleEmission);
                default:
                    return null;
            }
        }
    }
}