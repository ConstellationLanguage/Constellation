namespace Constellation.Sound {
    public class SoundNodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
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