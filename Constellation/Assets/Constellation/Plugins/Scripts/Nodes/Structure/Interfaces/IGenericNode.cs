namespace Constellation
{
    public interface IGenericNode
    {
        int[] GetGenericOutputByLinkedInput(int inputID);
        bool IsGenericInput(int inputID);
        int[] GetGenericInputByLinkedOutput(int outputID);
        bool IsGenericOutput(int outputID);
    }
}
