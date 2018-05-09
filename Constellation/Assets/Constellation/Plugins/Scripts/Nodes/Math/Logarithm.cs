using UnityEngine;

namespace Constellation.Math {
public class Logarithm: INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Logarithm";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "A");
            sender = _node.GetSender();
            _node.AddOutput(false, "Log(A)");
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
                sender.Send(new Variable().Set(Mathf.Log(_value.GetFloat())), 0);
        }
    }
}
