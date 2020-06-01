namespace Constellation
{
    [System.Serializable]
    public class LinkData
    {
        public InputData Input;
        public OutputData Output;
        public float outputPositionY;
        public string GUID = "";

        public LinkData(Link _link)
        {
            Input = new InputData(_link.Input.Guid, _link.Input.isBright, _link.Type, _link.Input.Description);
            Output = new OutputData(_link.Output.Guid, _link.Output.IsWarm, _link.Type, _link.Output.Description);
            GUID = _link.Guid;
        }

        public LinkData(InputData _input, OutputData _output)
        {
            Input = _input;
            Output = _output;
            GUID = System.Guid.NewGuid().ToString();
        }
    }
}