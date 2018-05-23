namespace Constellation
{

    public interface INode
    {
        void Setup(INodeParameters _node);
        string NodeName();
        string NodeNamespace();
        void Receive(Variable value, Input _input);
    }
}