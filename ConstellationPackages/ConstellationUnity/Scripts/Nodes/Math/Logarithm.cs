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

        public void Receive(Ray _value, Input _input)
        {

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.Log(_value.GetFloat())), 0);
        }
    }
}
