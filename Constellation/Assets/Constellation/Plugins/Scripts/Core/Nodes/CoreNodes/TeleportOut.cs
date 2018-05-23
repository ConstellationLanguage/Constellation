namespace Constellation.CoreNodes {
    public class TeleportOut : INode, IReceiver, ITeleportOut {
        public const string NAME = "TeleportOut";
        private Attribute eventName;
        private ITeleportIn teleport;

        public void Setup (INodeParameters _node) {
            _node.AddInput (this, true, "value to teleport");
            eventName = _node.AddAttribute (new Variable ("event name"), Attribute.AttributeType.Word, "The event name");
        }

        public void Set (ITeleportIn teleporter) {
            this.teleport = teleporter;
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            teleport.OnTeleport(value, this.eventName.Value.GetString());
        }
    }
}