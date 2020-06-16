using UnityEngine;

namespace Constellation.CoreNodes {
    public class ConstellationNodeFactory: INodeGetter{

        public string GetNameSpace()
        {
            return NameSpace.NAME;
        }

        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case Add.NAME:
                    INode nodeAdd = new Add () as INode;
                    return new Node<INode> (nodeAdd);
                case Condition.NAME:
                    INode nodeCondition = new Condition () as INode;
                    return new Node<INode> (nodeCondition);
                case Multiply.NAME:
                    INode nodeMultiply = new Multiply () as INode;
                    return new Node<INode> (nodeMultiply);
                case Note.NAME:
                    INode note = new Note () as INode;
                    return new Node<INode> (note);
                case Print.NAME:
                    INode NodePrint = new Print () as INode;
                    return new Node<INode> (NodePrint);
                case Remove.NAME:
                    INode NodeRemove = new Remove () as INode;
                    return new Node<INode> (NodeRemove);
                case Switch.NAME:
                    INode NodeSwitch = new Switch () as INode;
                    return new Node<INode> (NodeSwitch);
                case Value.NAME:
                    INode nodeValue = new Value () as INode;
                    return new Node<INode> (nodeValue);
                case Var.NAME:
                    INode nodeVar = new Var () as INode;
                    return new Node<INode> (nodeVar);
                case Word.NAME:
                    INode nodeWord = new Word () as INode;
                    return new Node<INode> (nodeWord);
                case Sender.NAME:
                    INode nodeSender = new Sender() as INode;
                    return new Node<INode> (nodeSender);
                case Receiver.NAME:
                    INode nodeReceiver = new Receiver() as INode;
                    return new Node<INode>(nodeReceiver);
                case CodeVar.NAME:
                    INode nodeGetVar = new CodeVar()  as INode;
                    return new Node<INode>(nodeGetVar);
                case TeleportIn.NAME:
                    INode nodeTeleportIn = new TeleportIn() as INode;
                    return new Node<INode>(nodeTeleportIn);
                case TeleportOut.NAME:
                    INode nodeTeleportOut = new TeleportOut() as INode;
                    return new Node<INode>(nodeTeleportOut);
                case Exit.NAME:
                    INode exit = new Exit() as INode;
                    return new Node<INode>(exit);
                case Entry.NAME:
                    INode entry = new Entry() as INode;
                    return new Node<INode>(entry);
                case BrightEntry.NAME:
                    INode brightEntry = new BrightEntry() as INode;
                    return new Node<INode>(brightEntry);
                case BrightExit.NAME:
                    INode brightExit = new BrightExit() as INode;
                    return new Node<INode>(brightExit);
                case IsUpdated.NAME:
                    INode isUpdated = new IsUpdated() as INode;
                    return new Node<INode>(isUpdated);
                default:
                    return null;
            }
        }
    }
}