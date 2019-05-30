namespace Constellation.Custom
{
    public class ConstellationNodeFactory : INodeGetter, IRequestAssembly
    {
        ConstellationScriptData[] constellationsScripts;

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode(string nodeName)
        {
            INode customNode = new CustomNode() as INode;
            return new Node<INode>(customNode);
        }

        public void SetConstellationAssembly(ConstellationScriptData [] _constellationScripts)
        {
            constellationsScripts = _constellationScripts;
        }
    }
}