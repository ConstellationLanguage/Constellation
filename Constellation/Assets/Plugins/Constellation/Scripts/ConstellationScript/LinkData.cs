namespace Constellation
{
    [System.Serializable]
    public class LinkData
    {
        public InputData Input;
        public OutputData Output;
        public float outputPositionY;
        public LinkData(Link _link)
        {   
           Input = new InputData(_link.Input.Guid, _link.Input.isWarm, _link.Type, _link.Input.Description);
           Output = new OutputData(_link.Output.Guid, _link.Output.IsWarm, _link.Type, _link.Output.Description);
        }

        public LinkData(InputData _input, OutputData _outPut){
           Input = _input;
           Output = _outPut;
        }
    }
}