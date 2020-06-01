namespace Constellation.CoreNodes
{
    public class Multiply : INode, IReceiver
    {
		private ISender sender;
        private Ray [] varsToAdd;
        private Ray result;
        public const string NAME = "Multiply";
        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, false, "Multiply factor");
			_node.AddInput(this, true, "Value to multiply");
            sender = _node.GetSender();
            _node.AddOutput(false, "Result $1 x $2");
            varsToAdd = new Ray[2];
            varsToAdd[0] = new Ray().Set(0);
            varsToAdd[1] = new Ray().Set(0);
            result = new Ray().Set(0);
        }


        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            if(_value.IsFloat())
                varsToAdd[_input.InputId].Set(_value.GetFloat());
            else 
                varsToAdd[_input.InputId].Set(_value.GetString());

            if( varsToAdd[0].IsFloat() &&  varsToAdd[1].IsFloat())
                result.Set(varsToAdd[0].GetFloat() * varsToAdd[1].GetFloat());

            if (_input.isBright)
                sender.Send(result, 0);
        }
	}
}
