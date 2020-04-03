using Constellation.Services;

namespace Constellation.CoreNodes
{
    public class Print : INode, IReceiver, IInjectLogger
    {
        private Services.ILogger logger;
        public const string NAME = "Print";
        public void Setup(INodeParameters _nodeParameters)
        {
            _nodeParameters.AddInput(this, false, "Any", "value to log in console");
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Ray value, Input _input)
        {
            if (value.IsFloat() || value.IsString())
            {
                logger.Log(value.GetString());
                return;
            }

             if (value.GetObject() != null)
                logger.Log(value.GetObject());
        }

        public void InjectLogger(Services.ILogger _logger)
        {
            logger = _logger;
        }
    }
}