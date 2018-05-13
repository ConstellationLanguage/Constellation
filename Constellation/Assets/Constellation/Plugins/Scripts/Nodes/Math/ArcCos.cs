using UnityEngine;

namespace Constellation.Math
{
    public class ArcCos : INode, IReceiver
    {
        private ISender sender;
        public const string NAME = "ArcCos";
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "a");
            sender = _node.GetSender();
            _node.AddOutput(false, "arc-cosine of a");
        }


        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            if (_input.isWarm)
                sender.Send(new Variable().Set(Mathf.Acos(_value.GetFloat())), 0);
        }
    }
}
