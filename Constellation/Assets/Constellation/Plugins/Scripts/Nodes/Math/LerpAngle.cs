using UnityEngine;

namespace Constellation.Math {
public class LerpAngle: INode, IReceiver
    {
		private ISender sender;
		private Ray startValue;
		private Ray endValue;
        public const string NAME = "LerpAngle";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "A");
			_node.AddInput(this, false, "B");
			_node.AddInput(this, true, "T");
            sender = _node.GetSender();
            _node.AddOutput(false, "Same as lerp but with radians");
			startValue = new Ray().Set(0);
			endValue = new Ray().Set(0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
			if(_input.InputId == 0)
				startValue.Set(_value.GetFloat());
			else if(_input.InputId == 1)
				endValue.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.LerpAngle(_value.GetFloat(), startValue.GetFloat(), endValue.GetFloat())), 0);
        }
    }
}
