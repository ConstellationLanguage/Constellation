namespace Constellation.CoreNodes {
    public class Value : INode, IReceiver, IAwakable {
        private ISender sender;
        private Parameter value;
        public const string NAME = "Value";

        public void Setup (INodeParameters _node) {
            var newValue = new Ray ().Set(0);
            sender = _node.GetSender();
            _node.AddOutput (true, "The value");
            value = _node.AddParameter (newValue, Parameter.ParameterType.Value, "Number to set");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnAwake () {
            sender.Send (value.Value, 0);
        }

        public void Receive (Ray _value, Input _input) {
            if (_value.IsFloat ()) {
                value.Value.Set (_value.GetFloat ());
            }
            if (_input.isBright)
                sender.Send (value.Value, 0);
        }
    }
}