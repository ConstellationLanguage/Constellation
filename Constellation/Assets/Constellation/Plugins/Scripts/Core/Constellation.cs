using System.Collections.Generic;

namespace Constellation
{
    [System.Serializable]
    public class Constellation : ConstellationObject
    {
        private List<Node<INode>> Nodes;
        public List<Link> Links;


        public override void Initialize(string _guid, string _name)
        {
            Injector = new Injector(this);
            base.Initialize(_guid, _name);
            if (Nodes == null)
                Nodes = new List<Node<INode>>();
        }

        public void SetConstellationEvents()
        {
            if(Injector == null)
                Injector = new Injector(this);
            Injector.SetConstellationEvents();
        }

        public void AddLink(LinkData link)
        {
            AddLink(new Link(GetInput(link.Input.Guid),
                GetOutput(link.Output.Guid),
                GetOutput(link.Output.Guid).Type), "none");
        }

        public Input GetInput(string guid)
        {
            foreach (Node<INode> node in Nodes)
            {
                if (node.Inputs != null)
                {
                    foreach (Input input in node.Inputs)
                    {

                        if (guid == input.Guid)
                        {
                            return input;
                        }
                    }
                }
            }
            //Debug.Log(guid + " not found for Input");
            return null;
        }

        public Node<INode>[] GetNodes()
        {
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            return Nodes.ToArray();
        }

        public Output GetOutput(string guid)
        {
            foreach (Node<INode> node in Nodes)
            {
                if (node.Outputs != null)
                {
                    foreach (Output output in node.Outputs)
                    {
                        if (guid == output.Guid)
                        {
                            return output;
                        }
                    }
                }
            }
            //Debug.Log(guid + " not found for Output");
            return null;
        }

        public Node<INode> AddNode(Node<INode> node, string guid, NodeData nodeData = null)
        {
            if (Nodes == null)
                Nodes = new List<Node<INode>>();

            var newNode = node;
            newNode.Initialize(guid, node.Name);
            if (nodeData != null)
            {
                newNode.XPosition = nodeData.XPosition;
                newNode.YPosition = nodeData.YPosition;
            }
            Nodes.Add(newNode);
            return newNode;
        }

        public Link[] GetLinks()
        {
            if (Links == null)
                Links = new List<Link>();

            return Links.ToArray();
        }

        public void RemovedNode(string guid)
        {
            foreach (var node in Nodes)
            {
                if (node.Guid == guid)
                {
                    var links = Links.ToArray();
                    foreach (var link in links)
                    {
                        foreach (var input in node.Inputs)
                        {
                            if (link.Input.Guid == input.Guid)
                            {
                                link.OnDestroy();
                                Links.Remove(link);
                            }
                        }
                        foreach (var output in node.Outputs)
                        {
                            if (link.Output.Guid == output.Guid)
                            {
                                link.OnDestroy();
                                Links.Remove(link);
                            }
                        }
                    }
                    node.Destroy();
                    Nodes.Remove(node);
                    return;
                }
            }
            //Debug.LogError("Constellation: Node not found");
        }

        public Link AddLink(Link link, string guid)
        {
            if (Links == null)
                Links = new List<Link>();

            var newLink = link;
            link.Initialize(guid, guid);
            Links.Add(link);

            return newLink;
        }
    }
}