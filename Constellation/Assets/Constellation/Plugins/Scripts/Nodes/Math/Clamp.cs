using UnityEngine;

namespace Constellation.Math {
public class Clamp: INode, IReceiver
    {
		private ISender sender;
		private Variable startValue;
		private Variable endValue;
        public const string NAME = "Clamp";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "min");
			_node.AddInput(this, false, "max");
			_node.AddInput(this, true, "t");
            sender = _node.GetSender();
            _node.AddOutput(false, "Clamps a value between a minimum value and maximum value");
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

            if (_input.isWarm)
                sender.Send(new Variable().Set(Mathf.Clamp(_value.GetFloat(), startValue.GetFloat(), endValue.GetFloat())), 0);
        }
    }
}
