using UnityEngine;

namespace Constellation.Math {
public class NextPowerOfTwo: INode, IReceiver
    {
		private ISender sender;
		private Ray Var1;
        public const string NAME = "NextPowerOfTwo";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "Var");
			Var1 = new Ray(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Next power of two");
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
				Var1.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set((float) Mathf.NextPowerOfTwo((int)Var1.GetFloat())), 0);
        }
    }
}
