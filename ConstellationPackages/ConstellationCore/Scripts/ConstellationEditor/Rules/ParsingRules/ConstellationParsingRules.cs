using System.Collections.Generic;
using System;
using System.Linq;

namespace Constellation
{
    public class ConstellationParsingRules
    {
        private NodesFactory NodesFactory;
        private List<IParsingRule> parsingRules;

        public void UpdateScriptsNodes(ConstellationScriptData[] staticConstellationNodes, ConstellationScriptData[] constellationScripts, IConstellationFileParser constellationFileParser)
        {
            SetParsingRules();
            foreach (var script in staticConstellationNodes)
            {
                foreach (var node in script.Nodes)
                {
                    if (node.Name == ConstellationTypes.StaticConstellationNode.NAME)
                    {
                        script.NameSpace = node.GetParameters()[0].Value.GetString();
                    }
                }
                UpdateScriptNodes(script, staticConstellationNodes, constellationFileParser);
            }

            foreach (var script in constellationScripts)
            {
                UpdateScriptNodes(script, staticConstellationNodes, constellationFileParser);
            }
        }

        void SetParsingRules()
        {
            parsingRules = new List<IParsingRule>();
            var type = typeof(IParsingRule);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            foreach (var t in types)
            {
                if (t.FullName != typeof(IParsingRule).FullName)
                {
                    var rule = Activator.CreateInstance(t) as IParsingRule;
                    parsingRules.Add(rule);
                }
            }
        }

        private IConstellationFileParser InitializeConstellationParser()
        {
            var type = typeof(IConstellationFileParser);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            var constructor = types.ElementAt(0).GetConstructors();
            var obj = constructor[0].Invoke(new object[] { }) as IConstellationFileParser;
            return obj;
        }

        public void UpdateScriptNodes(ConstellationScriptData script, ConstellationScriptData[] constellationScripts, IConstellationFileParser constellationFileParser)
        {
            List<NodeData> nodesToRemove = new List<NodeData>();
            NodesFactory = new NodesFactory(constellationScripts);
            foreach (var node in script.Nodes)
            {
                var nodeObject = NodesFactory.GetNodeSafeMode(node, constellationFileParser);
                foreach(var parsingRule in parsingRules)
                {
                    if (!parsingRule.isNodeValid(node, nodeObject, NodesFactory))
                    {
                        nodesToRemove.Add(node);
                        break;
                    }
                }
            }

            foreach (var node in nodesToRemove)
            {
                script.RemoveNode(node.Guid);
                var replacementNode = NodesFactory.GetNode(node.Name, node.Namespace, InitializeConstellationParser());
                if (replacementNode != null)
                {
                    replacementNode.XPosition = node.XPosition;
                    replacementNode.YPosition = node.YPosition;
                    replacementNode.XSize = node.SizeX;
                    replacementNode.YSize = node.SizeY;

                    if (node.ParametersData != null && replacementNode.NodeParameters != null)
                    {
                        if (node.ParametersData.Count == replacementNode.NodeParameters.Count)
                        {
                            for (var i = 0; i < replacementNode.NodeParameters.Count; i++)
                            {
                                replacementNode.NodeParameters[i].Value = new Ray(node.ParametersData[i].Value);
                            }
                        }
                    }


                    if (node.Inputs != null && replacementNode.Inputs != null)
                    {
                        if (node.Inputs.Count >= replacementNode.Inputs.Count)
                        {
                            for (var i = 0; i < replacementNode.Inputs.Count; i++)
                            {
                                replacementNode.Inputs[i].Guid = node.Inputs[i].Guid;
                            }
                        }
                        else
                        {
                            for (var i = 0; i < node.Inputs.Count; i++)
                            {
                                replacementNode.Inputs[i].Guid = node.Inputs[i].Guid;
                            }
                        }
                    }

                    if (node.Outputs != null && replacementNode.Outputs != null)
                    {
                        if (node.Outputs.Count >= replacementNode.Outputs.Count)
                        {
                            for (var i = 0; i < replacementNode.Outputs.Count; i++)
                            {
                                replacementNode.Outputs[i].Guid = node.Outputs[i].Guid;
                            }
                        }
                        else
                        {
                            for (var i = 0; i < node.Outputs.Count; i++)
                            {
                                replacementNode.Outputs[i].Guid = node.Outputs[i].Guid;
                            }
                        }
                    }
                    script.AddNode(new NodeData(replacementNode));
                }
                else
                {
                    script.RemoveNode(node.Guid);
                }
            }

            foreach (var link in script.Links)
            {
                //if()
            }
        }
    }
}