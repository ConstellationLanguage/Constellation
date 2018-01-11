namespace Constellation.Sound {
    public static class SoundNodeFactory {
        public static Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case AudioSource.NAME:
                    INode nodeAudioSource = new AudioSource () as INode;
                    return new Node<INode> (nodeAudioSource);
                default:
                    return null;
            }
        }
    }
}