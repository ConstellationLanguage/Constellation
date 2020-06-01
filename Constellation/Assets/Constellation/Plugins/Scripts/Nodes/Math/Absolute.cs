using UnityEngine;

namespace Constellation.Math
{
    public class Absolute  : INode, IReceiver
    {
		private ISender sender;
        public const string NAME = "Absolute";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "Value abs");
            sender = _node.GetSender();
            _node.AddOutput(false, "Absolute value");
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
                sender.Send(new Ray().Set(Mathf.Abs(_value.GetFloat())), 0);
        }
    }
}