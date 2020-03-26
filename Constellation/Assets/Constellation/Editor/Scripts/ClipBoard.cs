using System.Collections.Generic;
using Constellation;
using Constellation.Unity3D;

namespace ConstellationEditor
{
    [System.Serializable]
    public class ClipBoard
    {
        public List<NodeData> nodes;
        public List<LinkData> links;

        public void AddSelection(NodeView[] _nodes, LinkData[] _links)
        {
            nodes = new List<NodeData>();
            links = new List<LinkData>();
            foreach (var view in _nodes)
            {
                var newNode = new NodeData(view.NodeData);
                newNode.XPosition = view.NodeData.XPosition + 30;
                newNode.YPosition = view.NodeData.YPosition + 30;
                newNode.Guid = System.Guid.NewGuid().ToString();

                foreach (var input in newNode.Inputs)
                {
                    input.Guid = System.Guid.NewGuid().ToString();
                }

                foreach (var output in newNode.Outputs)
                {
                    output.Guid = System.Guid.NewGuid().ToString();
                }

                nodes.Add(newNode);
            }

            foreach (var link in _links)
            {
                InputData input = null;
                OutputData output = null;
                for (var i = 0; i < _nodes.Length; i++)
                {

                    for (var j = 0; j < _nodes[i].NodeData.GetInputs().Length; j++)
                    {
                        if (link.Input.Guid == _nodes[i].NodeData.GetInputs()[j].Guid)
                            input = nodes[i].GetInputs()[j];
                    }

                    for (var k = 0; k < _nodes[i].NodeData.GetOutputs().Length; k++)
                    {
                        if (link.Output.Guid == _nodes[i].NodeData.GetOutputs()[k].Guid)
                            output = nodes[i].GetOutputs()[k];
                    }
                }
                if (input != null && output != null)
                {
                    links.Add(new LinkData(input, output));
                }
            }
        }

        public NodeData[] PasteClipBoard(ConstellationScript constellation)
        {
            if (nodes == null)
                return null;

            if (nodes.Count == 0)
                return null;
            var pastedNodes = new List<NodeData>();
            var pastedLinks = new List<LinkData>();
            foreach (var node in nodes)
            {
                var newNode = new NodeData(node);
                newNode.XPosition = node.XPosition;
                newNode.YPosition = node.YPosition;
                pastedNodes.Add(newNode);
            }

            foreach (var link in links)
            {
                InputData input = null;
                OutputData output = null;
                for (var i = 0; i < nodes.Count; i++)
                {

                    for (var j = 0; j < nodes[i].GetInputs().Length; j++)
                    {
                        if (link.Input.Guid == nodes[i].GetInputs()[j].Guid)
                            input = nodes[i].GetInputs()[j];
                    }

                    for (var k = 0; k < nodes[i].GetOutputs().Length; k++)
                    {
                        if (link.Output.Guid == nodes[i].GetOutputs()[k].Guid)
                            output = nodes[i].GetOutputs()[k];
                    }

                }
                if (input != null && output != null)
                {
                    pastedLinks.Add(new LinkData(input, output));
                }
            }
            foreach (var node in pastedNodes)
            {
                constellation.AddNode(node);
            }

            foreach (var link in pastedLinks)
            {
                constellation.AddLink(link);
            }

            nodes = null;
            links = null;

            return pastedNodes.ToArray();
        }
    }
}