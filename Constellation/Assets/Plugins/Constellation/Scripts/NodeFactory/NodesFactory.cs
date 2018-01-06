using System.Collections.Generic;
using Constellation.Attributes;
using Constellation.BasicNodes;
using Constellation.Experimental;
using Constellation.Math;
using Constellation.Physics;
using Constellation.UI;
using Constellation.Unity;
using Constellation.Sound;

namespace Constellation {
    public static class NodesFactory {

        public static Node<INode> GetNode (string _nodeName, string _nodenamespaces) {
            switch (_nodenamespaces) {
                case BasicNodes.NameSpace.NAME:
                    return ConstellationNodeFactory.GetNode (_nodeName);
                case Unity.NameSpace.NAME:
                    return UnityNodeFactory.GetNode (_nodeName);
                case Math.NameSpace.NAME:
                    return MathNodeFactory.GetNode (_nodeName);
                case Physics.NameSpace.NAME:
                    return PhysicsNodeFactory.GetNode (_nodeName);
                case Attributes.NameSpace.NAME:
                    return AttributesNodeFactory.GetNode (_nodeName);
                case UI.NameSpace.NAME:
                    return UINodeFactory.GetNode (_nodeName);
                case Experimental.NameSpace.NAME:
                    return ExperimentalNodeFactory.GetNode (_nodeName);
                case Sound.NameSpace.NAME:
                    return SoundNodeFactory.GetNode(_nodeName);
                default:
                    return null;
            }
        }

        public static Node<INode> GetNode (NodeData _nodeData) {
            Node<INode> node = null;
            if (_nodeData.Namespace == "") {
                node = ConstellationNodeFactory.GetNode (_nodeData.Name);

                if (node == null)
                    node = UnityNodeFactory.GetNode (_nodeData.Name);

                if (node == null)
                    node = MathNodeFactory.GetNode (_nodeData.Name);
                if (node == null)
                    return null;

            } else {
                switch (_nodeData.Namespace) {
                    case BasicNodes.NameSpace.NAME:
                        node = ConstellationNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Unity.NameSpace.NAME:
                        node = UnityNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Math.NameSpace.NAME:
                        node = MathNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Physics.NameSpace.NAME:
                        node = PhysicsNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Attributes.NameSpace.NAME:
                        node = AttributesNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case UI.NameSpace.NAME:
                        node = UINodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Experimental.NameSpace.NAME:
                        node = ExperimentalNodeFactory.GetNode (_nodeData.Name);
                        break;
                    case Sound.NameSpace.NAME:
                         node = SoundNodeFactory.GetNode(_nodeData.Name);
                         break;
                    default:
                        return null;
                }
            }

            var i = 0;
            foreach (Input input in node.GetInputs ()) {
                input.Guid = _nodeData.GetInputs () [i].Guid;
                i++;
            }

            var j = 0;
            foreach (Output output in node.GetOutputs ()) {
                output.Guid = _nodeData.GetOutputs () [j].Guid;
                j++;
            }

            var a = 0;
            foreach (Attribute attribute in node.GetAttributes ()) {
                if (_nodeData.GetAttributes () [a].Value.IsFloat ())
                    attribute.Value.Set (_nodeData.GetAttributes () [a].Value.GetFloat ());
                else
                    attribute.Value.Set (_nodeData.GetAttributes () [a].Value.GetString ());
                a++;
            }
            return node;
        }

        public static string[] GetAllNamespaces (string[] _nodes) {
            List<string> allNamespaces = new List<string> ();
            foreach (var node in _nodes) {
                if (!allNamespaces.Contains (node.Split ('.') [1])) {
                    allNamespaces.Add (node.Split ('.') [1]);
                }
            }
            return allNamespaces.ToArray ();
        }

        public static string[] GetAllNodes () {
            List<string> allNodes = new List<string> (GenericNodeFactory.GetNodesType ());
            return allNodes.ToArray ();
        }
    }
}