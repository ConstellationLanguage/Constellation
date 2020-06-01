namespace Constellation.CoreNodes
{
    public class Entry : INode, IReceiver, IMirrorNode
    {
        private ISender sender;
        private Parameter parameter; // attributes are setted in the editor.
        public const string NAME = "Entry"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        public void Setup(INodeParameters _node)
        {
            var wordValue = new Ray();
            _node.AddOutput(false, "Any", "Output the entry received"); // setting a cold input
            parameter = _node.AddParameter(wordValue.Set("Var"), Parameter.ParameterType.Word, "The default word");// setting an attribute (Used only for the editor)
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
            sender.Send(parameter.Value, 0);
        }
    }
}