using System;

namespace Constellation {
    public interface IConstellationScript {
        void Set (ConstellationScriptData _script);
        NodeData AddNode (NodeData _node);
        NodeData AddNode (Node<INode> _node);
        void RemoveNode (NodeData _node);
        void RemoveNode (Node<INode> _node);
        LinkData[] GetLinks ();
        NodeData[] GetNodes ();
        bool IsLinkValid (LinkData _link);
        void AddLink (LinkData _link);
        LinkData AddLink (Link _link);
        void RemoveLink (LinkData _link);
        ConstellationScriptData GetData ();
        Object GetScriptObject();
    }
}