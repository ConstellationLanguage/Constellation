namespace Constellation.CoreNodes
{
    public class Multiply : INode, IReceiver
    {
		private ISender sender;
        private Variable [] varsToAdd;
        private Variable result;
        public const string NAME = "Multiply";
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "Multiply factor");
			_node.AddInput(this, true, "Value to multiply");
            sender = _node.GetSender();
            _node.AddOutput(false, "Result $1 x $2");
            varsToAdd = new Variable[2];
            varsToAdd[0] = new Variable().Set(0);
            varsToAdd[1] = new Variable().Set(0);
            result = new Variable().Set(0);
        }


        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            if(_value.IsFloat())
                varsToAdd[_input.InputId].Set(_value.GetFloat());
            else 
                varsToAdd[_input.InputId].Set(_value.GetString());

            if( varsToAdd[0].IsFloat() &&  varsToAdd[1].IsFloat())
                result.Set(varsToAdd[0].GetFloat() * varsToAdd[1].GetFloat());

            if (_input.isWarm)
                sender.Send(result, 0);
        }
	}
}
