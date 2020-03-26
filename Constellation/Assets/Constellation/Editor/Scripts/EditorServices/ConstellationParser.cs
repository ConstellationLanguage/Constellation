using System.Collections.Generic;
using Constellation;

namespace ConstellationEditor
{
    public class ConstellationParser
    {
        private NodesFactory NodesFactory;

        public void UpdateScriptsNodes(ConstellationScript[] scripts, ConstellationScriptData[] constellationScripts)
        {
            foreach (var script in scripts)
            {
                UpdateScriptNodes(script.script, constellationScripts);
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
                else if (node.Inputs.Count != nodeObject.Inputs.Count || node.Outputs.Count != nodeObject.Outputs.Count)
                {
                    nodesToRemove.Add(node);
                }
                else
                {
                    var foundDifference = false;
                    var i = 0;
                    foreach(var input in node.GetInputs())
                    {
                        if (input.Type != nodeObject.Inputs[i].Type || input.IsWarm != nodeObject.Inputs[i].isWarm)
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
                            if (output.Type != nodeObject.Outputs[i].Type || output.IsWarm != nodeObject.Outputs[i].IsWarm)
                            {
                                nodesToRemove.Add(node);
                                break;
                            }
                            i++;
                        }
                    }
                }
            }

            /*if (nodesToRemove.Count == 0)
                return;*/

            foreach (var node in nodesToRemove)
            {
                script.RemoveNode(node.Guid);
                var replacementNode = NodesFactory.GetNode(node.Name, node.Namespace);
                if (replacementNode != null)
                {
                    replacementNode.XPosition = node.XPosition;
                    replacementNode.YPosition = node.YPosition;
                    replacementNode.XSize = node.SizeX;
                    replacementNode.YSize = node.SizeY;

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

            foreach(var link in script.Links)
            {
                //if()
            }
        }
    }
}