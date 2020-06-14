namespace Constellation.Unity
{
    public class ObjectParameter : INode, IReceiver, IAwakable, IParameter
    {
        public const string NAME = "ObjectParameter";
        private ISender sender;
        private Ray UnityObject;
		private Parameter name;
        private Ray nameValue;

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender();
            _node.AddOutput(true, "Object", "Unity object");
            UnityObject = new Ray().Set(null as object);
            nameValue = new Ray().Set("Default");
			_node.AddParameter(nameValue, Parameter.ParameterType.Word, "Name in inspector");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void SetParameter (Ray var) {
            UnityObject.Set(var.GetObject());
        }

        public void OnAwake()
        {
            if(UnityObject != null)
                sender.Send(UnityObject, 0);
        }

        public void Receive(Ray _value, Input _input)
        {
            UnityObject.Set(_value);
            sender.Send(UnityObject, 0);
        }
    }
}