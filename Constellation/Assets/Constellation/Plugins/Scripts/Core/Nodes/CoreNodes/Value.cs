namespace Constellation.CoreNodes {
    public class Value : INode, IReceiver, IAwakable {
        private ISender sender;
        private Attribute value;
        public const string NAME = "Value";

        public void Setup (INodeParameters _node) {
            var newValue = new Variable ();
            sender = _node.GetSender();
            _node.AddOutput (true, "The value");
            value = _node.AddAttribute (newValue, Attribute.AttributeType.Value, "Number to set");
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

        public void Receive (Variable _value, Input _input) {
            if (_value.IsFloat ()) {
                value.Value.Set (_value.GetFloat ());
            }
            if (_input.isWarm)
                sender.Send (value.Value, 0);
        }
    }
}