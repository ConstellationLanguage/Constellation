namespace Constellation.CoreNodes
{
    public class Add : INode, IReceiver
    {
        public const string NAME = "Add";
		private ISender sender;
        private Variable [] varsToAdd;
        private Variable result;
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "value 1");
			_node.AddInput(this, true, "value 2");
            sender = _node.GetSender();
            _node.AddOutput(false, "value 1 + value 2");
            varsToAdd = new Variable[2];
            varsToAdd[0] = new Variable().Set(0);
            varsToAdd[1] = new Variable().Set(0);
            result = new Variable().Set(0);
        }

        public string NodeName() 
        {
            return Add.NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            if(_value.IsFloat())
                varsToAdd[_input.InputId].Set(_value.GetFloat());
            else 
                varsToAdd[_input.InputId].Set(_value.GetString());

            if( varsToAdd[0].IsFloat() &&  varsToAdd[1].IsFloat() && _input.isWarm)
                result.Set(varsToAdd[0].GetFloat() + varsToAdd[1].GetFloat());
            else if(_input.isWarm)
                result.Set(varsToAdd[0].GetString() + varsToAdd[1].GetString());

            if (_input.isWarm)
                sender.Send(result, 0);
        }
    }
}
