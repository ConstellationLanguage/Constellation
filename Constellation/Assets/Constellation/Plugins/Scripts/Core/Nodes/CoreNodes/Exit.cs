namespace Constellation.CoreNodes
{
    public class Exit : INode, IReceiver
    {
        private ISender sender;
        private Attribute attribute; // attributes are setted in the editor.
        public const string NAME = "Exit"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)

        public void Setup(INodeParameters _node)
        {
            var wordValue = new Variable();
            _node.AddInput(this, true, "Output the entry received"); // setting a cold input
            attribute = _node.AddAttribute(wordValue.Set("Var"), Attribute.AttributeType.Word, "The default word");// setting an attribute (Used only for the editor)
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
        public void Receive(Variable _value, Input _input)
        {
           
        }
    }
}