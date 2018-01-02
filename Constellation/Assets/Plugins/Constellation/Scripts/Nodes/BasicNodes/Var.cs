namespace Constellation.BasicNodes {
    public class Var : INode, IReceiver{
        private ISender sender;
        private Attribute value;
        public const string NAME = "Var";

        public void Setup (INodeParameters _node, ILogger _logger) {
            var wordValue = new Variable ();
            _node.AddInput (this, false, "New var");
            _node.AddInput (this, true, "Send var");
            sender = _node.AddOutput (false, "Current setted word");
            value = _node.AddAttribute (wordValue.Set("Var"), Attribute.AttributeType.ReadOnlyValue, "The default word");
        }


        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable _value, Input _input) {
            if (_input.InputId == 0)
                value.Value.Set (_value);

            if (_input.InputId == 1)
                sender.Send (value.Value, 0);
        }
    }
}