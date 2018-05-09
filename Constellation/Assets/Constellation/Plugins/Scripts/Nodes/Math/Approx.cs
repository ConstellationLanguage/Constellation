using UnityEngine;

namespace Constellation.Math
{
    public class Approx : INode, IReceiver
    {
        private ISender sender;
        private Variable VarToCompare;
        public const string NAME = "Approx";
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "a");
            _node.AddInput(this, true, "b");
            sender = _node.GetSender();
            _node.AddOutput(false, "true if +- the same");
            VarToCompare = new Variable().Set(0);
        }


        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            if (_input.InputId == 0)
            {
                VarToCompare.Set(_value.GetFloat());
            }
            
            if (!_input.isWarm)
                return;
            if(Mathf.Approximately(_value.GetFloat(), VarToCompare.GetFloat()))
                sender.Send(new Variable().Set(1), 0);
            else 
                sender.Send(new Variable().Set(0), 0);
        }
    }
}