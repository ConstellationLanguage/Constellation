using UnityEngine;

namespace Constellation.Math {
public class Log10: INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Log10";
        
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "A");
            sender = _node.GetSender();
            _node.AddOutput(false, "base 10 logarithm of A");
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
                sender.Send(new Variable().Set(Mathf.Log10(_value.GetFloat())), 0);
        }
    }
}
