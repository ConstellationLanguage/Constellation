
namespace Constellation {
    public class ConstellationEditable: ConstellationComponent {
        public void RemoveLink (LinkData linkData) {
            Link linkToRemove = null;
            foreach (var link in constellation.Links) {
                if (link.Input.Guid == linkData.Input.Guid && link.Output.Guid == linkData.Output.Guid) {
                    linkToRemove = link;
                }
            }
            linkToRemove.OnDestroy ();
            constellation.Links.Remove (linkToRemove);
        }

        public void AddLink (LinkData link) {
            constellation.AddLink (link);
        }

        public void RemoveNode (NodeData node) {
            constellation.RemovedNode (node.Guid);
        }
        
        public void AddNode (NodeData node) {
            var newNode = nodeFactory.GetNode (node);
            constellation.AddNode (newNode, node.Guid);
            AddUnityObject (newNode);
        }
    }
}