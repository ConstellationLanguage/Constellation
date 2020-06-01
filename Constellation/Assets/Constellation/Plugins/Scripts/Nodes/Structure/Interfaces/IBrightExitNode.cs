namespace Constellation
{
    public interface IBrightExitNode
    {
        Ray GetExitValue();
        void SubscribeReceiver(IRayReceiver receiver, int id);
    }
}
