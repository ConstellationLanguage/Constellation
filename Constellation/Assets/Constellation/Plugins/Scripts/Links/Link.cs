namespace Constellation
{
	[System.Serializable]
	public class Link: ConstellationObject, IReceiver
	{
		public Input Input;
		public Output Output;
		public string GUID;
		public string Type;

		public Link(Input _input, Output _output, string _type, string _guid)
		{
			Type = _type;
			Input = _input;
			Output = _output;
			Output.Register(this);		
			_input.isConnected = true;
			GUID = _guid;
		}

		public void Receive(Ray value, Input input){
			Input.Receive(value, Input);
		}

		public override void OnDestroy()
        {
			Input.isConnected = false;	
			Output.Unregister(this);
        }
	}
}
