namespace Constellation.CoreNodes
{
    public class Add : INode, IReceiver
    {
        public const string NAME = "Add";
		private ISender sender;
        private Ray [] varsToAdd;
        private Ray result;
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "value 1");
			_node.AddInput(this, true, "value 2");
            sender = _node.GetSender();
            _node.AddOutput(false, "value 1 + value 2");
            varsToAdd = new Ray[2];
            varsToAdd[0] = new Ray().Set(0);
            varsToAdd[1] = new Ray().Set(0);
            result = new Ray().Set(0);
        }

        public string NodeName() 
        {
            return Add.NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            if(_value.IsFloat())
                varsToAdd[_input.InputId].Set(_value.GetFloat());
            else 
                varsToAdd[_input.InputId].Set(_value.GetString());

            if( varsToAdd[0].IsFloat() &&  varsToAdd[1].IsFloat() && _input.isBright)
                result.Set(varsToAdd[0].GetFloat() + varsToAdd[1].GetFloat());
            else if(_input.isBright)
                result.Set(varsToAdd[0].GetString() + varsToAdd[1].GetString());

            if (_input.isBright)
                sender.Send(result, 0);
        }
    }
}
