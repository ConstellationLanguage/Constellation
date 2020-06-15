using System;
using System.Collections.Generic;
using System.Linq;

namespace Constellation
{
    [System.Serializable]
    public class NodesFactory
    {
        public List<INodeGetter> NodeGetters;
        public List<IRequestAssembly> AssemblyRequester;
        ConstellationScriptData[] staticConstellationScripts;

        public NodesFactory(ConstellationScriptData[] staticConstellationScripts)
        {
            this.staticConstellationScripts = staticConstellationScripts;
            Setup();
        }

        public ConstellationScriptData [] GetStaticScripts()
        {
            return staticConstellationScripts;
        }

        public ConstellationScriptData[] GetStaticConstellationScripts()
        {
            return staticConstellationScripts;
        }

        public void UpdateConstellatioNScripts(ConstellationScriptData [] newStaticConstellationList)
        {
            staticConstellationScripts = newStaticConstellationList;
            Setup();
        }

        private void Setup()
        {
            SetInterfaces(staticConstellationScripts);
        }

        public void SetInterfaces(ConstellationScriptData[] constellationScript)
        {
            AssemblyRequester = new List<IRequestAssembly>();
            NodeGetters = new List<INodeGetter>();
            var type = typeof(INodeGetter);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            foreach (var t in types)
            {
                if (t.FullName != "Constellation.INodeGetter")
                {
                    var factory = Activator.CreateInstance(t) as INodeGetter;
                    if(factory is IRequestAssembly)
                    {
                        (factory as IRequestAssembly).SetConstellationAssembly(constellationScript);
                        AssemblyRequester.Add(factory as IRequestAssembly);
                    }

                    NodeGetters.Add(factory);
                }
            }
        }
    
        public Node<INode> GetNode(string _nodeName, string _nodenamespaces, IConstellationFileParser jsonParser)
        {
            if (NodeGetters == null)
                Setup();

            if (_nodenamespaces.Contains("|" + ConstellationTypes.StaticConstellationNode.NAME +"|"))
            {
                _nodenamespaces = "Custom";
            }

            foreach (var nodesGetter in NodeGetters)
            {
                if (nodesGetter.GetNameSpace() == _nodenamespaces)
                {
                    var node = nodesGetter.GetNode(_nodeName);
                    if (node != null)
                        return node;
                }
            }
            return null;
        }

        public Node<INode> GetNodeSafeMode(NodeData _nodeData)
        {
            try
            {
                Node<INode> node = null;
                foreach (var nodesGetter in NodeGetters)
                {
                    if (nodesGetter.GetNameSpace() == _nodeData.Namespace)
                    {
                        var selectedNode = nodesGetter.GetNode(_nodeData.Name);
                        if (selectedNode != null)
                        {
                            node = selectedNode;
                            break;
                        }
                    }
                }

                var i = 0;
                foreach (Input input in node.GetInputs())
                {
                    input.Guid = _nodeData.GetInputs()[i].Guid;
                    i++;
                }

                var j = 0;
                foreach (Output output in node.GetOutputs())
                {
                    output.Guid = _nodeData.GetOutputs()[j].Guid;
                    j++;
                }

                var a = 0;
                foreach (Parameter attribute in node.GetParameters())
                {
                    if (_nodeData.GetParameters()[a].Value.IsFloat())
                        attribute.Value.Set(_nodeData.GetParameters()[a].Value.GetFloat());
                    else
                        attribute.Value.Set(_nodeData.GetParameters()[a].Value.GetString());
                    a++;
                }
                return node;
            }
            catch { return null; }

        }

        public Node<INode> GetNode(NodeData _nodeData)
        {
            Node<INode> node = null;
            foreach (var nodesGetter in NodeGetters)
            {
                if (nodesGetter.GetNameSpace() == _nodeData.Namespace)
                {
                    var selectedNode = nodesGetter.GetNode(_nodeData.Name);
                    if (selectedNode != null)
                    {
                        node = selectedNode;
                        break;
                    }
                }
            }
                
            var i = 0;
            foreach (Input input in node.GetInputs())
            {
                input.Guid = _nodeData.GetInputs()[i].Guid;
                i++;
            }

            var j = 0;
            foreach (Output output in node.GetOutputs())
            {
                output.Guid = _nodeData.GetOutputs()[j].Guid;
                j++;
            }

            var a = 0;
            foreach (Parameter parameter in node.GetParameters())
            {
                if (_nodeData.GetParameters()[a].Value.IsFloat())
                    parameter.Value.Set(_nodeData.GetParameters()[a].Value.GetFloat());
                else
                    parameter.Value.Set(_nodeData.GetParameters()[a].Value.GetString());
                a++;
            }
            return node;
        }

        public static string[] GetAllNamespaces(string[] _nodes)
        {
            List<string> allNamespaces = new List<string>();
            foreach (var node in _nodes)
            {
                if (!allNamespaces.Contains(node.Split('.')[1]))
                {
                    allNamespaces.Add(node.Split('.')[1]);
                }
            }
            return allNamespaces.ToArray();
        }

        public static string[] GetAllNodesExcludeDiscretes()
        {
            List<string> allNodes = new List<string>(GenericNodeFactory.GetNodesTypeExcludeDiscretes());
            return allNodes.ToArray();
        }
    }
}