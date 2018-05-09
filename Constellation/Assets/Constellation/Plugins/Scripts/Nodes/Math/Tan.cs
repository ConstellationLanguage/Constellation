using UnityEngine;

namespace Constellation.Math {
public class Tan: INode, IReceiver
    {
		private ISender sender;
		private Variable VarX;
        public const string NAME = "Tan";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "X");
			VarX = new Variable(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Tan of X");
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
				VarX.Set(_value.GetFloat());

            if (_input.isWarm)
                sender.Send(new Variable().Set(Mathf.Tan(VarX.GetFloat())), 0);
        }
    }
}
