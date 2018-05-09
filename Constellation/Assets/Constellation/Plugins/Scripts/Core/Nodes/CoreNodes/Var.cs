namespace Constellation.CoreNodes {
    public class Var : INode, IReceiver{
        private ISender sender;
        private Attribute attribute; // attributes are setted in the editor.
        public const string NAME = "Var"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        public void Setup (INodeParameters _node) {
            var wordValue = new Variable ();
            _node.AddInput (this, false, "New var"); // setting a cold input
            _node.AddInput (this, true, "Send var"); // setting a warm input
            sender = _node.GetSender();
            _node.AddOutput (false, "Current setted word"); // setting a cold input
            attribute = _node.AddAttribute (wordValue.Set("Var"), Attribute.AttributeType.ReadOnlyValue, "The default word");// setting an attribute (Used only for the editor)
        }

        //return the node name (used in the factory).
        public string NodeName () {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive (Variable _value, Input _input) {
            if (_input.InputId == 0)
                attribute.Value.Set (_value);

            if (_input.InputId == 1)
                sender.Send (attribute.Value, 0);
        }
    }
}