using Constellation.Services;

namespace Constellation.CoreNodes {
    public class Print : INode, IReceiver, IInjectLogger {
        private Services.ILogger logger;
        public const string NAME = "Print";
        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "value to log in console");
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

        public void InjectLogger(Services.ILogger _logger)
        {
            logger = _logger;
        }
    }
}