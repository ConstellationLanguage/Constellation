namespace Constellation.ConstellationTypes
{
    public class StaticConstellationNode : INode, IReceiver
    {
        public const string NAME = "StaticConstellationNode";
        private Parameter parameter; // attributes are setted in the editor.

        public void Setup(INodeParameters _node)
        {
            var wordValue = new Ray();
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
        }
    }
}