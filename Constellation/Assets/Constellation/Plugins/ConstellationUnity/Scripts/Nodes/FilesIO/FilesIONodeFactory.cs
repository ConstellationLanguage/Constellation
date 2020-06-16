namespace Constellation.FilesIO {
    public class FilesIONodeFactory: INodeGetter {

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }


        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case StreamingAssetsPath.NAME:
                    INode streamingAssetsPath = new StreamingAssetsPath() as INode;
                    return new Node<INode>(streamingAssetsPath);
                case LoadTextFileAtPath.NAME:
                    INode loadTextFileAtPath = new LoadTextFileAtPath() as INode;
                    return new Node<INode>(loadTextFileAtPath);
                case LoadResource.NAME:
                    INode loadResource = new LoadResource() as INode;
                    return new Node<INode>(loadResource);
                default:
                    return null;
            }
        }
    }
}