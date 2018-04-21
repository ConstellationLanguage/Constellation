using Constellation;

namespace ConstellationEditor {
    public interface ILinkEditor {
        void AddLinkFromInput(InputData selectedInput);
        void AddLinkFromOutput(OutputData selectedOutput);
    }
}