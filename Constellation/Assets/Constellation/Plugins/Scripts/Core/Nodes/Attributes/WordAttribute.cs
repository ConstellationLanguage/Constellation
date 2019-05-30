namespace Constellation.Attributes
{
    public class WordAttribute : INode, IReceiver, IAwakable, IAttribute
    {
        private ISender sender;
        private Variable defaultValue;
        private Variable Word;

        public const string NAME = "WordAttribute";

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender(); 
            _node.AddOutput(true, "Current value");
            defaultValue = new Variable().Set("Default");
			var nameValue = new Variable().Set("AttributeName");
			_node.AddAttribute(nameValue, Attribute.AttributeType.Word, "Attribute Name");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }


        public void SetAttribute (Variable var) {
            defaultValue.Set(var.GetString());
        }

        public void OnAwake()
        {
            sender.Send(defaultValue, 0);
        }

        public void Receive(Variable _value, Input _input)
        {
            defaultValue.Set(_value);
            sender.Send(defaultValue, 0);
        }
    }
}
