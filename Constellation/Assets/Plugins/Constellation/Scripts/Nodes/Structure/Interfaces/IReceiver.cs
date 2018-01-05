namespace Constellation
{

    public interface IReceiver
    {
        void Receive(Variable value, Input input);
    }
}
