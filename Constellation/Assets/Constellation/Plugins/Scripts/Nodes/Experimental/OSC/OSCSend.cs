namespace Constellation.Experimental {
    public class OSCSend : INode, IReceiver {
        private Attribute ChannelName; // attributes are setted in the editor.
        public const string NAME = "OSCSend"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        public void Setup (INodeParameters _node, ILogger _logger) {
            var wordValue = new Variable ();
            _node.AddInput (this, false, "Send OSC"); // setting a cold input
            ChannelName = _node.AddAttribute (wordValue.Set ("/SenderName"), Attribute.AttributeType.Word, "/Channel"); // setting an attribute (Used only for the editor)
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            if (OSCManager.OSC != null){
                OSCComponent.OscMessage message = new OSCComponent.OscMessage ();
                message.address = ChannelName.Value.GetString();
                message.values.Add (value.GetFloat());
                OSCManager.OSC.Send (message);
            }
        }
    }
}