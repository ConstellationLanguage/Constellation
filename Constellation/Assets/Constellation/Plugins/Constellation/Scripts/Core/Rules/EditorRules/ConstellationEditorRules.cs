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
                if (t.FullName != "Constellation.IConstellationEditorRule")
                {
                    var rule = Activator.CreateInstance(t) as IConstellationEditorRule;
                    constellationRules.Add(rule);
                }
            }
        }

        public NodeData AddNode(NodesFactory nodesFactory, string nodeName, string nameSpace, ConstellationScriptData constellationScript)
        {
            var newNode = nodesFactory.GetNode(nodeName, nameSpace, null);
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
            foreach (var constellationRule in constellationRules)
            {
                if (_output != null && _output.Type == UNDEFINED && _input != null && _input.Type != UNDEFINED)
                    return false;

                if (constellationRule.IsTypeValid(_input, _output))
                {

                    if (constellationRule.IsLinkValid(newLink, constellationScript))
                    {
                        linkIsValid();
                        constellationScript.AddLink(newLink);
                        linkCreated(newLink.GUID);
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateGenericNodeByLinkGUID(ConstellationScriptData constellationScript, NodesFactory nodesFactory, string guid)
        {
            foreach (var constellationRule in constellationRules)
            {
                constellationRule.UpdateGenericNodeByLinkGUID(constellationScript, nodesFactory, guid);
            }
        }

        public void RemoveNode(NodeData node, ConstellationScriptData constellationScript)
        {
            foreach (var constellationRule in constellationRules)
            {
                constellationRule.RemoveNode(node, constellationScript);
            }
        }
    }
}
