namespace Constellation.Experimental {
    public class OSCReceive : INode, IReceiver {
        private ISender sender;
        private Attribute ChannelName; // attributes are setted in the editor.
        public const string NAME = "OSCReceive"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        void OnReceive (OSCComponent.OscMessage message) {
            sender.Send (new Variable (message.GetFloat (0)), 0);
        }
        
        public void Setup (INodeParameters _node, ILogger _logger) {
            var wordValue = new Variable ();
            _node.AddInput (this, false, "On OSC Ready"); // setting a cold input
            sender = _node.GetSender();
            _node.AddOutput (true, "Current OSC Output"); // setting a cold input
            ChannelName = _node.AddAttribute (wordValue.Set ("/ReceiveName"), Attribute.AttributeType.Word, "/Channel"); // setting an attribute (Used only for the editor)
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            OSCManager.OSC.SetAddressHandler (ChannelName.Value.GetString (), OnReceive);
        }
    }
}