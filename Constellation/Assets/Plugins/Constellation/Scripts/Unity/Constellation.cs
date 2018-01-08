using System.Collections.Generic;
using UnityEngine;

namespace Constellation {
    [System.Serializable]
    public class Constellation : ConstellationObject {
        private List<Node<INode>> Nodes;
        public List<Link> Links;

        public override void Initialize (string _guid) {
            base.Initialize (_guid);
            if (Nodes == null)
                Nodes = new List<Node<INode>> ();
            Name = "Constellation";
        }

        public Input GetInput (string guid) {
            foreach (Node<INode> node in Nodes) {
                if (node.Inputs != null) {
                    foreach (Input input in node.Inputs) {

                        if (guid == input.Guid) {
                            return input;
                        }
                    }
                }
            }
            Debug.Log (guid + " not found for Input");
            return null;
        }

        public Node<INode>[] GetNodes () {
            if (Nodes == null)
                Nodes = new List<Node<INode>> ();

            return Nodes.ToArray ();
        }

        public Output GetOutput (string guid) {
            foreach (Node<INode> node in Nodes) {
                if (node.Outputs != null) {
                    foreach (Output output in node.Outputs) {
                        if (guid == output.Guid) {
                            return output;
                        }
                    }
                }
            }
            Debug.Log (guid + " not found for Output");
            return null;
        }

        public Node<INode> AddNode (Node<INode> node, string guid) {
            if (Nodes == null)
                Nodes = new List<Node<INode>> ();

            var newNode = node;
            node.Initialize (guid);
            Nodes.Add (node);
            return newNode;
        }

        public void RemovedNode (string guid) {
            foreach (var node in Nodes) {
                if (node.Guid == guid) {
                    var links = Links.ToArray();
                    foreach (var link in links) {
                        foreach (var input in node.Inputs) {
                            if (link.Input.Guid == input.Guid) {
                                link.OnDestroy();
                                Links.Remove (link);
                            }
                        }
                        foreach (var output in node.Outputs) {
                            if (link.Output.Guid == output.Guid) {
                                link.OnDestroy();
                                Links.Remove (link);
                            }
                        }
                    }
                    node.Destroy();
                    Nodes.Remove (node);
                    return;
                }
            }
            Debug.LogError ("Constellation: Node not found");
        }

        public Link AddLink (Link link, string guid) {
            if (Links == null)
                Links = new List<Link> ();

            var newLink = link;
            link.Initialize (guid);
            Links.Add (link);

            return newLink;
        }
    }
}