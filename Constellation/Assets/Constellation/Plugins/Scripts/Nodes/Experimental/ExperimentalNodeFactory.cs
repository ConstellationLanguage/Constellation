namespace Constellation.Experimental {
    public class ExperimentalNodeFactory: INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case OSCManager.NAME:
                    INode nodeOsc = new OSCManager () as INode;
                    return new Node<INode> (nodeOsc);
                case OSCReceive.NAME:
                    INode nodeOSCFilter = new OSCReceive () as INode;
                    return new Node<INode> (nodeOSCFilter);
                case OSCSend.NAME:
                    INode nodeOSCSend = new OSCSend () as INode;
                    return new Node<INode> (nodeOSCSend);
                case DesktopCapture.NAME:
                    INode nodeDestopCapture = new DesktopCapture() as INode;
                    return new Node<INode> (nodeDestopCapture);
                default:
                    return null;
            }
        }
    }
}