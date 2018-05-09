using UnityEngine;

namespace Constellation.Math
{
    public class Ceil  : INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Ceil";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "a");
            sender = _node.GetSender();
            _node.AddOutput(false, "Smallest integer greater to or equal to a");
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
                sender.Send(new Variable().Set(Mathf.Ceil(_value.GetFloat())), 0);
        }
    }
}
