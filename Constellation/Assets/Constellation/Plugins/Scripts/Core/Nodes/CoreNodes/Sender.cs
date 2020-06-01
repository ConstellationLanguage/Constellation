namespace Constellation.CoreNodes {
    public class Sender : INode, IReceiver {
        public const string NAME = "Sender";
        private Parameter eventName;

        public void Setup (INodeParameters _node) {
            _node.AddInput (this, true, "value to send");
            eventName = _node.AddParameter (new Ray ("event name"), Parameter.ParameterType.Word, "The event name");

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray value, Input _input) {
            if (_input.isBright)
                Constellation.eventSystem.SendEvent (eventName.Value.GetString(), value);
        }
    }
}