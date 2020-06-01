using UnityEngine;

namespace Constellation.Math {
public class SquareRoot: INode, IReceiver
    {
		private ISender sender;
		private Ray VarX;
        public const string NAME = "SquareRoot";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "X");
			VarX = new Ray(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Square Root of X");
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
				VarX.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.Sqrt(VarX.GetFloat())), 0);
        }
    }
}
