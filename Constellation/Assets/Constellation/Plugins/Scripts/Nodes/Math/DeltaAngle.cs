using UnityEngine;

namespace Constellation.Math
{
    public class DeltaAngle : INode, IReceiver
    {
        private ISender sender;
        private Ray variable;
        public const string NAME = "DeltaAngle";
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "Angle a");
            _node.AddInput(this, true, "Angle b");
            sender = _node.GetSender();
            _node.AddOutput(false, "Sortest difference between a and b");
            variable = new Ray().Set(0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            if (_input.InputId == 0)
            {
                variable.Set(_value.GetFloat());
            }

            if (_input.isBright)
                sender.Send(new Ray().Set(Mathf.DeltaAngle(_value.GetFloat(), variable.GetFloat())), 0);
        }
    }
}
