namespace Constellation.Unity
{
    public class ObjectAttribute : INode, IReceiver, IAwakable, IAttribute
    {
        public const string NAME = "ObjectAttribute";
        private ISender sender;
        private Variable UnityObject;
		private Attribute name;
        private Variable nameValue;

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender();
            _node.AddOutput(true, "Object", "Unity object");
            UnityObject = new Variable().Set(null as object);
            nameValue = new Variable().Set("Default");
			_node.AddAttribute(nameValue, Attribute.AttributeType.Word, "Name in inspector");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void SetAttribute (Variable var) {
            UnityObject.Set(var.GetObject());
        }

        public void OnAwake()
        {
            if(UnityObject != null)
                sender.Send(UnityObject, 0);
        }

        public void Receive(Variable _value, Input _input)
        {
            UnityObject.Set(_value);
            sender.Send(UnityObject, 0);
        }
    }
}