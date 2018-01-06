namespace Constellation.Experimental {
    public static class ExperimentalNodeFactory {
        public static Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case OSCManager.NAME:
                    INode nodeOsc = new OSCManager () as INode;
                    return new Node<INode> (nodeOsc);
                case OSCFilter.NAME:
                    INode nodeOSCFilter = new OSCFilter () as INode;
                    return new Node<INode> (nodeOSCFilter);
                default:
                    return null;
            }
        }
    }
}