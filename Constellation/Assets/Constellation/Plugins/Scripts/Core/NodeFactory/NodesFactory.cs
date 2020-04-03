using System;
using System.Collections.Generic;
using System.Linq;

namespace Constellation
{
    [System.Serializable]
    public class NodesFactory
    {
        public static NodesFactory Current;
        public List<INodeGetter> NodeGetters;
        public List<IRequestAssembly> AssemblyRequester;
        ConstellationScriptData[] scripts;

        public NodesFactory(ConstellationScriptData[] constellationScripts)
        {
            scripts = constellationScripts;
            Setup();
        }

        private void Setup()
        {
            Current = this;
            SetConstellationAssembly(scripts);
            SetInterfaces();
        }

        public void SetConstellationAssembly(ConstellationScriptData[] constellationScript)
        {
            if (constellationScript == null)
                return;
            AssemblyRequester = new List<IRequestAssembly>();
            var type = typeof(IRequestAssembly);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            foreach (var t in types)
            {
                if (t.FullName != "Constellation.IRequestAssembly")
                {
                    var factory = Activator.CreateInstance(t) as IRequestAssembly;
                    factory.SetConstellationAssembly(constellationScript);
                    AssemblyRequester.Add(factory);
                }
            }
        }

        public void SetInterfaces()
        {
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
                    NodeGetters.Add(factory);
                }
            }
        }
    
        public Node<INode> GetNode(string _nodeName, string _nodenamespaces)
        {
            if (NodeGetters == null)
                Setup();

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
            //throw new ConstellationNotAddedToFactory();
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
                foreach (Parameter attribute in node.GetAttributes())
                {
                    if (_nodeData.GetAttributes()[a].Value.IsFloat())
                        attribute.Value.Set(_nodeData.GetAttributes()[a].Value.GetFloat());
                    else
                        attribute.Value.Set(_nodeData.GetAttributes()[a].Value.GetString());
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
            foreach (Parameter attribute in node.GetAttributes())
            {
                if (_nodeData.GetAttributes()[a].Value.IsFloat())
                    attribute.Value.Set(_nodeData.GetAttributes()[a].Value.GetFloat());
                else
                    attribute.Value.Set(_nodeData.GetAttributes()[a].Value.GetString());
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

        public static string[] GetAllNodes()
        {
            List<string> allNodes = new List<string>(GenericNodeFactory.GetNodesType());
            return allNodes.ToArray();
        }
    }
}