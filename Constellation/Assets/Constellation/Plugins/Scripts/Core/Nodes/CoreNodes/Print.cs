namespace Constellation.CoreNodes {
    public class Print : INode, IReceiver {
        private ILogger logger;
        public const string NAME = "Print";
        public void Setup (INodeParameters _nodeParameters, ILogger _logger) {
            _nodeParameters.AddInput (this, false, "value to log in console");
            logger = _logger;
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            logger.Log (value);
        }
    }
}