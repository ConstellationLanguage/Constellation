using UnityEngine;

namespace Constellation.Math {
public class Max: INode, IReceiver
    {
		private ISender sender;
		private Variable Var1;
		private Variable Var2;
        public const string NAME = "Max";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "Var1");
			_node.AddInput(this, true, "Var2");
			Var1 = new Variable(0);
			Var2 = new Variable(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Max value");
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
				Var1.Set(_value.GetFloat());
			else if(_input.InputId == 1)
				Var2.Set(_value.GetFloat());

            if (_input.isWarm)
                sender.Send(new Variable().Set(Mathf.Max(Var1.GetFloat(), Var2.GetFloat())), 0);
        }
    }
}
