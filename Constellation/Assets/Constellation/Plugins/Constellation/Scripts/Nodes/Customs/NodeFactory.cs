
namespace Constellation.ConstellationNodes
{
    public class ConstellationNodeFactory : INodeGetter, IRequestAssembly
    {
        ConstellationScriptData[] constellationsScripts;

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode(string nodeName, IConstellationFileParser constellationFileParser)
        {
            INode customNode = new StaticConstellation() as INode;
            var node = new Node<INode>(customNode);
            
            node.GetDiscreteParameters()[0].Value = new Ray().Set(nodeName);
            foreach(var script in constellationsScripts)
            {
                if(script.Name == nodeName)
                {
                    (customNode as ICustomNode).UpdateNode(script, constellationFileParser);
                    (customNode as ICustomNode).SetupNodeIO(constellationFileParser);
                    return node;
                }
            }
            return node;
        }

        public void SetConstellationAssembly(ConstellationScriptData [] _constellationScripts)
        {
            constellationsScripts = _constellationScripts;
        }
    }
}