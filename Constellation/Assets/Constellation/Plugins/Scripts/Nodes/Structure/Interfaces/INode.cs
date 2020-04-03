namespace Constellation
{

    public interface INode
    {
        void Setup(INodeParameters _node);
        string NodeName();
        string NodeNamespace();
        void Receive(Ray value, Input _input);
    }
}