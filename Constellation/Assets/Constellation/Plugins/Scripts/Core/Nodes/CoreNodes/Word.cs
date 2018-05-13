namespace Constellation.CoreNodes {
    public class Word : INode, IReceiver, IAwakable {
        private ISender sender;
        private Attribute value;
        public const string NAME = "Word";

        public void Setup (INodeParameters _node) {
            var newValue = new Variable ().Set("your word");
            sender = _node.GetSender();
            _node.AddOutput (true, "The Word");
            value = _node.AddAttribute (newValue, Attribute.AttributeType.Word, "Word to set");
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
            value.Value.Set (_value.GetString ());
            if (_input.isWarm)
                sender.Send (value.Value, 0);
        }
    }
}