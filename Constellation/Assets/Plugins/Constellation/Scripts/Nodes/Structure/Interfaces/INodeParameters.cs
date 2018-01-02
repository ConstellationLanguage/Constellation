namespace Constellation
{
    public interface INodeParameters
    {
        Input AddInput(IReceiver receiver, bool isWarm, string description);
        Input AddInput(IReceiver receiver, bool isWarm, string type, string description);
        ISender AddOutput(bool isWarm, string description);
        ISender AddOutput(bool isWarm, string type, string description);
        Attribute AddAttribute(Variable value, Attribute.AttributeType _type, string description);
    }
}