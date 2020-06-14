using UnityEngine;

namespace Constellation.Math {
public class ArcTan2: INode, IReceiver
    {
		private ISender sender;
		private Ray Variable;
        public const string NAME = "ArcTan2";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "x");
			_node.AddInput(this, true, "y");
            sender = _node.GetSender();
            _node.AddOutput(false, "Angle between the x-axis and a 2D vector starting at zero and terminating at (x,y)");
            Variable = new Ray().Set(0);
			
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
				Variable.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.Atan2(_value.GetFloat(), Variable.GetFloat())), 0);
        }
    }
}
