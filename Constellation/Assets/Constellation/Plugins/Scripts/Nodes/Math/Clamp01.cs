using UnityEngine;

namespace Constellation.Math {
public class Clamp01: INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Clamp01";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "a");
            sender = _node.GetSender();
            _node.AddOutput(false, "Clamps value between 0 and 1 and returns value.");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.Clamp01(_value.GetFloat())), 0);
        }
    }
}
