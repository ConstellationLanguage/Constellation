using UnityEngine;

namespace Constellation.Math {
public class ArcSin: INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "ArcSin";
        
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "a");
            sender = _node.GetSender(); 
            _node.AddOutput(false, "the arc-sine of a");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            if (_input.isWarm)
                sender.Send(new Variable().Set(Mathf.Asin(_value.GetFloat())), 0);
        }
    }
}
