namespace Constellation.UserInputs
{
    public class KeyDown : INode, IReceiver, IUpdatable
    {
		public const string NAME = "KeyDown";
		private Parameter key;
		private ISender sender;
		private Ray keyState;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Ray();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "1 on key down else 0");
			key = _nodeParameters.AddParameter(newValue, Parameter.ParameterType.Word, "Key code");
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
			if(UnityEngine.Input.GetKeyDown(key.Value.GetString())){
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
