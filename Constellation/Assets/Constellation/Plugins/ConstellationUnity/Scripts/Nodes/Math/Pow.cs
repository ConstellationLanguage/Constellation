using UnityEngine;

namespace Constellation.Math {
public class Pow: INode, IReceiver
    {
		private ISender sender;
		private Ray VarF;
		private Ray VarP;
        public const string NAME = "Pow";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "F");
			_node.AddInput(this, true, "P");
			VarF = new Ray(0);
			VarP = new Ray(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "F raised to power P");
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
				VarF.Set(_value.GetFloat());
			
			if(_input.InputId == 1)
				VarP.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.Pow(VarF.GetFloat(), VarP.GetFloat())), 0);
        }
    }
}
