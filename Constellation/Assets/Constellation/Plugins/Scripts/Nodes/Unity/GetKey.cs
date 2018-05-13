namespace Constellation.Unity
{
    public class Key : INode, IReceiver, IUpdatable
    {
		public const string NAME = "Key";
        public Transform transform;
		private Attribute key;
		private ISender sender;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Variable();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "Output 1 when key pressed");
			key = _nodeParameters.AddAttribute(newValue, Attribute.AttributeType.Word, "The key");
        }
        
        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

		public void OnUpdate()
		{
			if(UnityEngine.Input.GetKey(key.Value.GetString()))
				sender.Send(new Variable().Set(1), 0);
			else if(UnityEngine.Input.GetKeyUp(key.Value.GetString())) {
				sender.Send(new Variable().Set(0), 0);
			}
		}

        public void Receive(Variable value, Input _input)
        {
        }
    }
}
