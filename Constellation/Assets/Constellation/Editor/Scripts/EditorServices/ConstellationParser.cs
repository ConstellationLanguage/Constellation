using System.Collections.Generic;
using Constellation;
//using UnityEngine;

namespace ConstellationEditor
{
    public class ConstellationParser
    {
        private NodesFactory NodesFactory;

        public void UpdateScriptsNodes(ConstellationScriptData[] staticConstellationNodes, ConstellationScriptData[] constellationScripts)
        {

            foreach (var script in staticConstellationNodes)
            {
                foreach (var node in script.Nodes)
                {
                    if (node.Name == Constellation.ConstellationTypes.StaticConstellationNode.NAME)
                    {
                        script.NameSpace = node.GetParameters()[0].Value.GetString();
                    }
                }
                UpdateScriptNodes(script, staticConstellationNodes);
            }

            foreach (var script in constellationScripts)
            {
                UpdateScriptNodes(script, staticConstellationNodes);
            }
        }

        public void UpdateScriptNodes(ConstellationScriptData script, ConstellationScriptData[] constellationScripts)
        {
            List<NodeData> nodesToRemove = new List<NodeData>();
            NodesFactory = new NodesFactory(constellationScripts);
            foreach (var node in script.Nodes)
            {
                var nodeObject = NodesFactory.GetNodeSafeMode(node);


                if (nodeObject == null)
                {
                    nodesToRemove.Add(node);
                }
                else if (node.Inputs.Count != nodeObject.Inputs.Count || node.Outputs.Count != nodeObject.Outputs.Count || node.GetParameters().Length != nodeObject.GetParameters().Length)
                {
                    nodesToRemove.Add(node);
                }
                else if (node.Namespace == Constellation.ConstellationNodes.NameSpace.NAME) // to be done only when the node is edited
                {
                    nodesToRemove.Add(node);
                }
                else
                {
                    var foundDifference = false;
                    var i = 0;
                    foreach (var input in node.GetInputs())
                    {
                        if ((input.Type != nodeObject.Inputs[i].Type && nodeObject.Inputs[i].Type != ConstellationEditorRules.ANY && nodeObject.Inputs[i].Type != ConstellationEditorRules.GENERIC && nodeObject.Inputs[i].Type != ConstellationEditorRules.UNDEFINED) || input.IsBright != nodeObject.Inputs[i].isBright || input.Description != nodeObject.Inputs[i].Description)
                        {
                            nodesToRemove.Add(node);
                            foundDifference = true;
                            break;
                        }
                        i++;
                    }

                    if (!foundDifference)
                    {
                        i = 0;
                        foreach (var output in node.GetOutputs())
                        {
                            if ((output.Type != nodeObject.Outputs[i].Type && nodeObject.Outputs[i].Type != ConstellationEditorRules.ANY && nodeObject.Outputs[i].Type != ConstellationEditorRules.GENERIC && nodeObject.Outputs[i].Type != ConstellationEditorRules.UNDEFINED) || output.IsBright != nodeObject.Outputs[i].IsWarm || output.Description != nodeObject.Outputs[i].Description)
                            {
                                nodesToRemove.Add(node);
                                break;
                            }
                            i++;
                        }
                    }

                    if (!foundDifference)
                    {
                        i = 0;
                        if (node.GetParameters().Length != nodeObject.GetParameters().Length)
                        {
                            nodesToRemove.Add(node);
                        }

                        foreach (var parameter in node.GetParameters())
                        {
                            if (parameter.Type != nodeObject.GetParameters()[i].Type)
                            {
                                nodesToRemove.Add(node);
                                break;
                            }
                            i++;
                        }
                    }
                }
            }

            foreach (var node in nodesToRemove)
            {
                script.RemoveNode(node.Guid);
                var replacementNode = NodesFactory.GetNode(node.Name, node.Namespace, null);
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