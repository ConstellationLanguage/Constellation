namespace Constellation.CoreNodes
{
    public class Exit : INode, IReceiver, IMirrorNode, IExitNode
    {
        private ISender sender;
        private Parameter attribute; // attributes are setted in the editor.
        public const string NAME = "Exit"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        private Ray exitValue;

        public void Setup(INodeParameters _node)
        {
            var wordValue = new Ray();
            _node.AddInput(this, false, "Any", "Output the entry received"); // setting a cold input
            attribute = _node.AddParameter(wordValue.Set("Var"), Parameter.ParameterType.Word, "The default word");// setting an attribute (Used only for the editor)
            exitValue = new Ray();
        }

        //return the node name (used in the factory).
        public string NodeName()
        {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive(Ray _value, Input _input)
        {
            exitValue = new Ray(_value);
        }

        public Ray GetExitValue()
        {
            return exitValue;
        }
    }
}