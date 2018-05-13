namespace Constellation.Unity
{
    public class KeyDown : INode, IReceiver, IUpdatable
    {
		public const string NAME = "KeyDown";
        public Transform transform;
		private Attribute key;
		private ISender sender;
		private Variable keyState;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Variable();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "1 on key down else 0");
			key = _nodeParameters.AddAttribute(newValue, Attribute.AttributeType.Word, "Key code");
			keyState = new Variable().Set(0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

		public void OnUpdate()
		{
			if(UnityEngine.Input.GetKeyDown(key.Value.GetString())){
				sender.Send(keyState.Set(1), 0);
			} else {
				sender.Send(keyState.Set(0), 0);
			}
		}

        public void Receive(Variable value, Input _input)
        {
        }
    }
}
