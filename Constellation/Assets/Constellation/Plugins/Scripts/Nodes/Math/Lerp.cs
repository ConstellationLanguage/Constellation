using UnityEngine;

namespace Constellation.Math {
public class Lerp: INode, IReceiver
    {
		private ISender sender;
		private Variable startValue;
		private Variable endValue;
        public const string NAME = "Lerp";
        public void Setup(INodeParameters _node, ILogger _logger)
        {
			_node.AddInput(this, false, "A");
			_node.AddInput(this, false, "B");
			_node.AddInput(this, true, "T");
            sender = _node.AddOutput(false, "T=0 output A, T=1 output B");
			startValue = new Variable().Set(0);
			endValue = new Variable().Set(0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
			if(_input.InputId == 0)
				startValue.Set(_value.GetFloat());
			else if(_input.InputId == 1)
				endValue.Set(_value.GetFloat());

            if (_input.isWarm) {
                sender.Send(new Variable().Set(Mathf.Lerp(startValue.GetFloat(), endValue.GetFloat(), _value.GetFloat())), 0);
            }
        }
    }
}
