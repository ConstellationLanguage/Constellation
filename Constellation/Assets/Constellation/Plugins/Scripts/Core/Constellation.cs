using System.Collections.Generic;
using UnityEngine;

namespace Constellation {
    [System.Serializable]
    public class Constellation : ConstellationObject, ITeleportIn {
        private List<Node<INode>> Nodes;
        public List<Link> Links;
        public List<IUpdatable> updatables;
        public List<IAwakable> Awakables;
        public List<ILateUpdatable> lateUpdatables;
        public List<ITeleportIn> teleportsIn;
        public List<ITeleportOut> teleportsOut;

        public override void Initialize (string _guid, string _name) {
            base.Initialize (_guid, _name);
            if (Nodes == null)
                Nodes = new List<Node<INode>> ();
        }

        public void Awake () {
            foreach (var awakables in Awakables) {
                awakables.OnAwake ();
            }
        }

        public void SetAwakables () {
            if (Awakables == null)
                Awakables = new List<IAwakable> ();

            foreach (var node in GetNodes ()) {
                if (node.NodeType as IAwakable != null) {
                    Awakables.Add (node.NodeType as IAwakable);
                }
            }
        }

        public void SetUpdatables () {
            if (updatables == null)
                updatables = new List<IUpdatable> ();

            foreach (var node in GetNodes ()) {
                if (node.NodeType as IUpdatable != null) {
                    updatables.Add (node.NodeType as IUpdatable);
                }
            }
        }

        public void OnTeleport (Variable
            var, string id) {
            foreach (var teleport in teleportsIn) {
                teleport.OnTeleport (var, id);
            }
        }

        public void Update () {
            if (updatables == null)
                return;

            foreach (var updatable in updatables) {
                updatable.OnUpdate ();
            }
        }

        public void LateUpdate () {
            foreach (var lateUpdatable in lateUpdatables) {
                lateUpdatable.OnLateUpdate ();
            }
        }

        public void SetLateUpdatables () {
            if (lateUpdatables == null)
                lateUpdatables = new List<ILateUpdatable> ();

            foreach (var node in GetNodes ()) {
                if (node.NodeType as ILateUpdatable != null) {
                    lateUpdatables.Add (node.NodeType as ILateUpdatable);
                }
            }
        }

        public void SubscribeUpdate (IUpdatable _updatable) {
            updatables.Add (_updatable);
        }

        public void RemoveUpdatable (IUpdatable _updatable) {
            updatables.Remove (_updatable);
        }

        public void RefreshConstellationEvents () {
            updatables = null;
            Awakables = null;
            lateUpdatables = null;
            SetConstellationEvents ();
        }

        public void SetConstellationEvents () {
            SetTeleports ();
            SetUpdatables ();
            SetLateUpdatables ();
            SetAwakables ();
        }

        public void AddLink (LinkData link) {
            AddLink (new Link (GetInput (link.Input.Guid),
                GetOutput (link.Output.Guid),
                GetOutput (link.Output.Guid).Type), "none");
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

        public void SetTeleports () {
            SetTeleportsIn ();
            SetTeleportsOut ();
        }

        public void SetTeleportsIn () {

            if (teleportsIn == null)
                teleportsIn = new List<ITeleportIn> ();

            foreach (var node in GetNodes ()) {
                if (node.NodeType as ITeleportIn != null) {
                    var newTeleportIn = node.NodeType as ITeleportIn;
                    teleportsIn.Add (newTeleportIn);
                }
            }
        }

        public void SetTeleportsOut () {

            if (teleportsOut == null)
                teleportsOut = new List<ITeleportOut> ();

            foreach (var node in GetNodes ()) {
                if (node.NodeType as ITeleportOut != null) {
                    var newTeleportOut = node.NodeType as ITeleportOut;
                    teleportsOut.Add (newTeleportOut);
                    newTeleportOut.Set (this);
                }
            }
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

        public Node<INode> AddNode (Node<INode> node, string guid, NodeData nodeData = null) {
            if (Nodes == null)
                Nodes = new List<Node<INode>> ();

            var newNode = node;
            newNode.Initialize (guid, node.Name);
            if (nodeData != null) {
                newNode.XPosition = nodeData.XPosition;
                newNode.YPosition = nodeData.YPosition;
            }
            Nodes.Add (newNode);
            return newNode;
        }

        public Link[] GetLinks () {
            if(Links == null)
                Links = new List<Link>();

            return Links.ToArray ();
        }

        public void RemovedNode (string guid) {
            foreach (var node in Nodes) {
                if (node.Guid == guid) {
                    var links = Links.ToArray ();
                    foreach (var link in links) {
                        foreach (var input in node.Inputs) {
                            if (link.Input.Guid == input.Guid) {
                                link.OnDestroy ();
                                Links.Remove (link);
                            }
                        }
                        foreach (var output in node.Outputs) {
                            if (link.Output.Guid == output.Guid) {
                                link.OnDestroy ();
                                Links.Remove (link);
                            }
                        }
                    }
                    node.Destroy ();
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
            link.Initialize (guid, guid);
            Links.Add (link);

            return newLink;
        }
    }
}