namespace Constellation
{
    public interface IRayReceiver
    {
        void SendRay(Ray ray, int id);
    }
}