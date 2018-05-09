namespace Constellation.CoreNodes {
    public class Switch : INode, IReceiver {
        private Variable switchValue;
        private ISender sender;
        public const string NAME = "Switch";
        public void Setup (INodeParameters nodeParameters) {
            nodeParameters.AddInput (this, false, "1 = on, 0 = off");
            nodeParameters.AddInput (this, true, "Value to send");
            sender = nodeParameters.GetSender();
            nodeParameters.AddOutput (false, "Output if on");
            switchValue = new Variable (0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            if (switchValue.GetFloat () == 1 && _input.isWarm)
                sender.Send (value, 0);
            else if (!_input.isWarm)
                switchValue.Set (value.GetFloat ());

        }
    }
}