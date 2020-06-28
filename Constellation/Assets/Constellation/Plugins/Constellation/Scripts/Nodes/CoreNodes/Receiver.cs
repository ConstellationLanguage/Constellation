namespace Constellation.CoreNodes {
    public class Receiver : INode, IReceiver, IDestroy{
        public const string NAME = "Receiver";
        private ISender sender;
        private Parameter eventName;
        public void Setup (INodeParameters _node) {
            sender = _node.GetSender();
            _node.AddOutput (true, "Received Value from a sender");
            eventName = _node.AddParameter (new Ray ("event name"), Parameter.ParameterType.Word, "The event name");
            if (Constellation.eventSystem != null)
                Constellation.eventSystem.Register (OnConstellationEvent);
        }

        public void OnConstellationEvent (string _eventName, Ray _value) {
            if (_eventName == this.eventName.Value.GetString ()) {
                sender.Send (_value, 0);
            }
        }

        public void OnDestroy()
        {
            Constellation.eventSystem.Unregister(OnConstellationEvent);
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