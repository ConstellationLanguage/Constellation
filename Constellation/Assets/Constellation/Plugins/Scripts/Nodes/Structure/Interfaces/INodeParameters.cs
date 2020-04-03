namespace Constellation
{
    public interface INodeParameters
    {
        Input AddInput(IReceiver receiver, bool isWarm, string description);
        Input AddInput(IReceiver receiver, bool isWarm, string type, string description);
        void AddOutput(bool isWarm, string description);
        void AddOutput(bool isWarm, string type, string description);
        Parameter AddAttribute(Ray value, Parameter.AttributeType _type, string description);
        ISender GetSender();
    }
}