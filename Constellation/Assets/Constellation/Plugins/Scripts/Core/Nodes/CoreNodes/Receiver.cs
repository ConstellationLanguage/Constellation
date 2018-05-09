namespace Constellation.CoreNodes {
    public class Receiver : INode, IReceiver, IDestroy{
        public const string NAME = "Receiver";
        private ISender sender;
        private Attribute eventName;
        public void Setup (INodeParameters _node) {
            sender = _node.GetSender();
            _node.AddOutput (true, "Received Value from a sender");
            eventName = _node.AddAttribute (new Variable ("event name"), Attribute.AttributeType.Word, "The event name");
            if (ConstellationComponent.eventSystem != null)
                ConstellationComponent.eventSystem.Register (OnConstellationEvent);
        }

        public void OnConstellationEvent (string _eventName, Variable _value) {
            if (_eventName == this.eventName.Value.GetString ()) {
                sender.Send (_value, 0);
            }
        }

        public void OnDestroy()
        {
            ConstellationComponent.eventSystem.Unregister(OnConstellationEvent);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {

        }
    }
}