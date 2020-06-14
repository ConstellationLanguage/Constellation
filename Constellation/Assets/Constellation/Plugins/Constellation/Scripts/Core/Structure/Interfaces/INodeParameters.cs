namespace Constellation
{
    public interface INodeParameters
    {
        Input AddInput(IReceiver receiver, bool isWarm, string description);
        Input AddInput(IReceiver receiver, bool isWarm, string type, string description);
        void AddOutput(bool isWarm, string description);
        void AddOutput(bool isWarm, string type, string description);
        Parameter AddParameter(Ray value, Parameter.ParameterType _type, string description);
        Parameter AddDiscreteParameter(Ray value, Parameter.ParameterType _type, string description);
        ISender GetSender();
    }
}