using System;
using System.Collections.Generic;
using System.Linq;

namespace Constellation
{
    public class ConstellationEditorRules
    {
        public const string UNDEFINED = "Undefined";
        public const string ANY = "Any";
        public const string GENERIC = "Generic";
        public const string VAR = "Var";
        public delegate void LinkValid();
        public delegate void LinkAdded(string linkGuid);
        private List<IConstellationEditorRule> constellationRules;

        public ConstellationEditorRules()
        {
            SetAllConstellationRules();
        }

        private void SetAllConstellationRules()
        {
            constellationRules = new List<IConstellationEditorRule>();
            var type = typeof(IConstellationEditorRule);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            foreach (var t in types)
            {
                if (t.FullName != typeof(IConstellationEditorRule).FullName)
                {
                    var rule = Activator.CreateInstance(t) as IConstellationEditorRule;
                    constellationRules.Add(rule);
                }
            }
        }

        public NodeData AddNode(NodesFactory nodesFactory, string nodeName, string nameSpace, ConstellationScriptData constellationScript)
        {
            var newNode = nodesFactory.GetNode(nodeName, nameSpace, new UnityConstellationParser());
            var nodeData = new NodeData(newNode);
            foreach (var constellationRule in constellationRules)
            {
                nodeData = constellationRule.NodeAdded(nodeData, newNode, constellationScript);
            }

            nodeData = constellationScript.AddNode(nodeData);
            nodeData.XPosition = 0;
            nodeData.YPosition = 0;

            return nodeData;
        }

        public bool AddLink(InputData _input, OutputData _output, ConstellationScriptData constellationScript, ConstellationEditorRules.LinkValid linkIsValid, ConstellationEditorRules.LinkAdded linkCreated)
        {
            var newLink = new LinkData(_input, _output);
            if (_output == null || _input == null)
                return false;

            foreach (var constellationRule in constellationRules)
            {
                if (constellationRule.IsLinkValid(_input, _output))
                {
                    if (!IsLinkValid(newLink, constellationScript))
                    {
                        return false;
                    }
                } else if(_input != null && _output != null)
                {
                    return false;
                }
            }
            linkIsValid();
            constellationScript.AddLink(newLink);
            linkCreated(newLink.GUID);
            return true;
        }

        public void UpdateGenericNodeByLinkGUID(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid, IConstellationFileParser constellationParser)
        {
            foreach (var constellationRule in constellationRules)
            {
                constellationRule.LinkAdded(constellationScript, nodesFactory, guid, constellationParser);
            }
        }

        public void RemoveNode(NodeData node, ConstellationScriptData constellationScript)
        {
            foreach (var constellationRule in constellationRules)
            {
                if (!constellationRule.CanRemoveNode(node, constellationScript))
                    return;
            }
            constellationScript.RemoveNode(node.Guid);
        }

        public bool IsLinkValid(LinkData _link, ConstellationScriptData _constellationScriptData)
        {
            foreach (LinkData link in _constellationScriptData.Links)
            {
                if (_link.Input.Guid == link.Input.Guid && _link.Output.Guid == link.Output.Guid)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
