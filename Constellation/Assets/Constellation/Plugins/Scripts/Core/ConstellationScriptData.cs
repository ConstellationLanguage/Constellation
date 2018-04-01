using System.Collections.Generic;

namespace Constellation {
    [System.Serializable]
    public class ConstellationScriptData {
        public List<NodeData> Nodes;
        public List<LinkData> Links;

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
    }
}