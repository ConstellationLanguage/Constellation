namespace Constellation.CoreNodes {
    public class Switch : INode, IReceiver, IGenericNode
    {
        private Ray switchValue;
        private ISender sender;
        public const string NAME = "Switch";
        public void Setup (INodeParameters nodeParameters) {
            nodeParameters.AddInput (this, false, "1 = on, 0 = off");
            nodeParameters.AddInput (this, true, "Any", "Value to send");
            sender = nodeParameters.GetSender();
            nodeParameters.AddOutput (false, "Any", "Output if on");
            switchValue = new Ray (0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray value, Input _input) {
            if (switchValue.GetFloat () == 1 && _input.isBright)
                sender.Send (value, 0);
            else if (!_input.isBright)
                switchValue.Set (value.GetFloat ());

        }

        public int[] GetGenericOutputByLinkedInput(int inputID)
        {
            if (inputID == 1)
            {
                return new int[1] { 0 };
            }
            return new int[0];
        }

        public bool IsGenericInput(int inputID)
        {
            if(inputID == 1)
            {
                return true;
            }
            return false;
        }

        public int[] GetGenericInputByLinkedOutput(int outputID)
        {
            if (outputID == 0)
            {
                return new int[1] { 1 };
            }
            return new int[0];
        }

        public bool IsGenericOutput(int outputID)
        {
            if (outputID == 0)
                return true;

            return false;
        }
    }
}