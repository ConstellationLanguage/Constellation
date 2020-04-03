namespace Constellation.Attributes {
    public class ValueParameter : INode, IReceiver, IAwakable, IAttribute{
        private ISender sender;
        private Ray Value;
        public const string NAME = "ValueParameter";

        public void Setup (INodeParameters _node) {
            sender = _node.GetSender();
            _node.AddOutput (true, "Current value");
            var newValue = new Ray (0);
            var nameValue = new Ray ("AttributeName");
            Value = newValue;
            _node.AddAttribute (nameValue, Parameter.AttributeType.Word, "Attribute name");

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void SetAttribute (Ray var) {
            Value.Set (var.GetFloat ());
        }

        public void OnAwake () {
            sender.Send (Value, 0);
        }

        public void Receive (Ray _value, Input _input) {
            if (_value.IsFloat ()) {
              Value.Set (_value.GetFloat ());
            }
            sender.Send (Value, 0);
        }
    }
}