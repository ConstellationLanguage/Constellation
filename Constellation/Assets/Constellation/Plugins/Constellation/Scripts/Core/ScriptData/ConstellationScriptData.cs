using System.Collections.Generic;
using Constellation.CoreNodes;

namespace Constellation {
    [System.Serializable]
    public class ConstellationScriptData {
        public List<NodeData> Nodes;
        public List<LinkData> Links;
        public string Name;
        public string NameSpace;
        const string EntryNodeName = Entry.NAME;
        const string ExitNodeName = Exit.NAME;

        public ConstellationScriptData Set (ConstellationScriptData script) {
            Nodes = null;
            Links = null;
            foreach (var node in script.Nodes) {
                AddNode (node);
            }
            foreach (var link in script.Links) {
                AddLink (link);
            }
            return this;
        }

        public NodeData [] GetAllEntryNodes()
        {
            var entryNodes = new List<NodeData>();

            foreach(var node in Nodes)
            {
                if(node.Name == EntryNodeName)
                {
                    entryNodes.Add(node);
                }
            }
            return entryNodes.ToArray();
        }

        public NodeData[] GetAllExitNodes()
        {
            var exitNodes = new List<NodeData>();

            foreach (var node in Nodes)
            {
                if (node.Name == ExitNodeName)
                {
                    exitNodes.Add(node);
                }
            }
            return exitNodes.ToArray();
        }

        public void RemoveNode(string guid)
        {
            foreach(var node in Nodes) {
                if(node.Guid == guid){
                    Nodes.Remove(node);
                    return;
                }
            }
        }

        public NodeData FindNodeByGUID (string _guid) {
            if (Nodes == null)
                Nodes = new List<NodeData> ();

            foreach (var node in Nodes) {
                if (node.Guid == _guid) {
                    return node;
                }
            }
            return null;
        }

        public LinkData[] FindLinksByInputGUID (string _inputGUID) {
            if (Links == null)
                Links = new List<LinkData> ();

            List<LinkData> links = new List<LinkData> ();

            foreach (var link in Links) {
                if (link.Input.Guid == _inputGUID) {
                    links.Add (link);
                }
            }
            return links.ToArray ();
        }

        public LinkData[] FindLinksByOutputGUID (string _outputGuid) {
            if (Links == null)
                Links = new List<LinkData> ();

            List<LinkData> links = new List<LinkData> ();

            foreach (var link in Links) {
                if (link.Output.Guid == _outputGuid) {
                    links.Add (link);
                }
            }
            return links.ToArray ();
        }

        public NodeData AddNode (NodeData _node) {
            if (Nodes == null)
                Nodes = new List<NodeData> ();
            var newNode = new NodeData (_node);
            newNode.XPosition = _node.XPosition;
            newNode.YPosition = _node.YPosition;
            Nodes.Add (newNode);
            return newNode;
        }

        public void AddLink (LinkData _link) {
            if (Links == null)
                Links = new List<LinkData> ();
            Links.Add (_link);
        }

        public NodeData[] GetNodes()
        {
            if (Nodes == null)
                Nodes = new List<NodeData>();

            return Nodes.ToArray();
        }

        public LinkData[] GetLinks()
        {
            if (Links == null)
                Links = new List<LinkData>();

            return Links.ToArray();
        }

        public NodeData[] GetNodesWithLinkGUID(string guid, out int inputId, out int outputId)
        {
            NodeData InputNode = null;
            NodeData OutputNode = null;
            inputId = 0;
            outputId = 0;
            foreach (var link in GetLinks())
            {
                if (link.GUID == guid)
                {
                    foreach (var node in GetNodes())
                    {
                        var inputCounter = 0;
                        foreach (var input in node.Inputs)
                        {
                            if (link.Input.Guid == input.Guid)
                            {
                                InputNode = node;
                                inputId = inputCounter;
                            }
                            inputCounter++;
                        }

                        var outputCounter = 0;
                        foreach (var output in node.Outputs)
                        {
                            if (link.Output.Guid == output.Guid)
                            {
                                outputId = outputCounter;
                                OutputNode = node;
                            }
                            outputCounter++;
                        }
                    }
                }
            }

            return new NodeData[2] { OutputNode, InputNode };
        }
    }
}