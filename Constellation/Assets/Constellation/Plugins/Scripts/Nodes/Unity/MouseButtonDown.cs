namespace Constellation.Unity
{
    public class MouseButtonDown : INode, IReceiver, IUpdatable
    {
		public const string NAME = "MouseButtonDown";
        public Transform transform;
		private Attribute key;
		private ISender sender;
		private Variable keyState;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Variable();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "1 on mouse button down else 0");
			key = _nodeParameters.AddAttribute(newValue, Attribute.AttributeType.Value, "mouse button code");
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
			if(UnityEngine.Input.GetMouseButtonDown((int)key.Value.GetFloat())){
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
