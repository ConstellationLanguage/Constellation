using Constellation;

namespace ConstellationUnityEditor {
    public interface ILinkEditor {
        void AddLinkFromInput(InputData selectedInput);
        void AddLinkFromOutput(OutputData selectedOutput);
    }
}