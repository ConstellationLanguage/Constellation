namespace Constellation.CoreNodes {
    public class Var : INode, IReceiver, IGenericNode
    {
        private ISender sender;
        private Parameter attribute; // attributes are setted in the editor.
        public const string NAME = "Var"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        public void Setup (INodeParameters _node) 
        {
            var wordValue = new Ray();
            _node.AddInput (this, false, "Generic", "New var"); // setting a cold input
            _node.AddInput (this, true, "Any", "Send var"); // setting a warm input
            sender = _node.GetSender();
            _node.AddOutput (false, "Generic", "Output var"); // setting a cold input
            attribute = _node.AddParameter (wordValue.Set("Var"), Parameter.ParameterType.ReadOnlyValue, "The default word");// setting an attribute (Used only for the editor)
        }

        //return the node name (used in the factory).
        public string NodeName () 
        {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace () 
        {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive (Ray _value, Input _input) 
        {
            if (_input.InputId == 0)
                attribute.Value.Set (_value);

            if (_input.InputId == 1)
                sender.Send (attribute.Value, 0);
        }

        public int [] GetGenericOutputByLinkedInput(int inputID)
        {
            if(inputID == 0)
            {
                return new int[1] { 0 };
            }
            return new int[0];
        }

        public bool IsGenericOutput(int inputID)
        {
            if (inputID == 0)
                return true;

            return false;
        }

        public bool IsGenericInput(int outputID)
        {
            if (outputID == 0)
                return true;

            return false;
        }

        public int [] GetGenericInputByLinkedOutput(int outputID)
        {
            if(outputID == 0)
            {
                return new int[1] { 0 };
            }
            return new int[0];
        }
    }
}