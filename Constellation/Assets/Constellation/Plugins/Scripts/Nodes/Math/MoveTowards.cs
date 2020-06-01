using UnityEngine;

namespace Constellation.Math {
public class MoveTowards: INode, IReceiver
    {
		private ISender sender;
		private Ray Var1;
		private Ray Var2;
		private Ray Var3;
        public const string NAME = "MoveTowards";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "Current");
			_node.AddInput(this, false, "Target");
			_node.AddInput(this, true, "MaxDelta");
			Var1 = new Ray(0);
			Var2 = new Ray(0);
			Var3 = new Ray(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Move a value toward target");
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
			else if(_input.InputId == 1)
				Var2.Set(_value.GetFloat());
			else if(_input.InputId == 2)
				Var3.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.MoveTowards(Var1.GetFloat(), Var2.GetFloat(), Var3.GetFloat())), 0);
        }
    }
}
