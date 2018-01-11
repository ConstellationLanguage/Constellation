namespace Constellation
{

    public interface INode
    {
        void Setup(INodeParameters _node, ILogger _logger);
        string NodeName();
        string NodeNamespace();
        void Receive(Variable value, Input _input);
    }
}