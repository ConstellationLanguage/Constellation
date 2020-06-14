namespace Constellation.Parameters
{
    public class WordParameter : INode, IReceiver, IAwakable, IParameter
    {
        private ISender sender;
        private Ray defaultValue;
        private Ray Word;

        public const string NAME = "WordParameter";

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender(); 
            _node.AddOutput(true, "Current value");
            defaultValue = new Ray().Set("Default");
			var nameValue = new Ray().Set("ParameterName");
			_node.AddParameter(nameValue, Parameter.ParameterType.Word, "Parameter Name");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }


        public void SetParameter (Ray var) {
            defaultValue.Set(var.GetString());
        }

        public void OnAwake()
        {
            sender.Send(defaultValue, 0);
        }

        public void Receive(Ray _value, Input _input)
        {
            defaultValue.Set(_value);
            sender.Send(defaultValue, 0);
        }
    }
}
