namespace Constellation.UserInputs
{
    public class MouseButtonDown : INode, IReceiver, IUpdatable
    {
		public const string NAME = "MouseButtonDown";
		private Parameter key;
		private ISender sender;
		private Ray keyState;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Ray();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "1 on mouse button down else 0");
			key = _nodeParameters.AddParameter(newValue, Parameter.ParameterType.Value, "mouse button code");
			keyState = new Ray().Set(0);
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

        public void Receive(Ray value, Input _input)
        {
        }
    }
}
