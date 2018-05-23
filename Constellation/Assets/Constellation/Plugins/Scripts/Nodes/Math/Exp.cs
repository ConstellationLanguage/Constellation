using UnityEngine;

namespace Constellation.Math {
public class Exp: INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Exp";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "A");
            sender = _node.GetSender();
            _node.AddOutput(false, "e raised to A power");
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
                sender.Send(new Variable().Set(Mathf.Exp(_value.GetFloat())), 0);
        }
    }
}
