namespace Constellation.Experimental {
    public static class ExperimentalNodeFactory {
        public static Node<INode> GetNode (string nodeName) {
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
                default:
                    return null;
            }
        }
    }
}