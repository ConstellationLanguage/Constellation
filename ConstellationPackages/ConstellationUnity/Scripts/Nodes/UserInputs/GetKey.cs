namespace Constellation.UserInputs
{
    public class Key : INode, IReceiver, IUpdatable
    {
		public const string NAME = "Key";
		private Parameter key;
		private ISender sender;
        public void Setup(INodeParameters _nodeParameters)
        {
			var newValue = new Ray();
			sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "Output 1 when key pressed");
			key = _nodeParameters.AddParameter(newValue, Parameter.ParameterType.Word, "The key");
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
				sender.Send(new Ray().Set(1), 0);
			else if(UnityEngine.Input.GetKeyUp(key.Value.GetString())) {
				sender.Send(new Ray().Set(0), 0);
			}
		}

        public void Receive(Ray value, Input _input)
        {
        }
    }
}
