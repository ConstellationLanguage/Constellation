namespace Constellation.CoreNodes
{
    public class Note : INode, IReceiver
    {
        public const string NAME = "Note";
        public void Setup(INodeParameters _node)
        {
            _node.AddParameter(new Ray().Set("Your note here"), Parameter.ParameterType.NoteField, "");
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME; 
        }

        
		public void Receive(Ray _value, Input _input)
		{

		}
    }
}
