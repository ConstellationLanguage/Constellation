namespace Constellation.CoreNodes {
    public class TeleportIn : INode, IReceiver, ITeleportIn {
        public const string NAME = "TeleportIn";
        private Parameter eventName;
        private ISender sender;

        public void Setup (INodeParameters _node) {
            _node.AddOutput (false, "Value received in the teleport");
            sender = _node.GetSender ();
            eventName = _node.AddParameter (new Ray ("event name"), Parameter.ParameterType.Word, "The event name");
        }

        public void OnTeleport (Ray variable, string id) {
            if (id == eventName.Value.GetString () || eventName.Value.GetString () == "")
                sender.Send (variable, 0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray value, Input _input) {

        }
    }
}