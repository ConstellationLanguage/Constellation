namespace Constellation.BasicNodes {
    public class TeleportOut : INode, IReceiver {
        public const string NAME = "TeleportOut";
        private Attribute eventName;

        public void Setup (INodeParameters _node, ILogger _logger) {
            _node.AddOutput (false, "Value received in the teleport");
            eventName = _node.AddAttribute (new Variable ("event name"), Attribute.AttributeType.Word, "The event name");
        }
    

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            if (_input.isWarm)
                ConstellationBehaviour.eventSystem.SendEvent (eventName.Value.GetString(), value);
        }
    }
}