namespace Constellation
{
	[System.Serializable]
	public class Link: ConstellationObject, IReceiver
	{
		public Input Input;
		public Output Output; 
		public string Type;

		public Link(Input _input, Output _output, string _type)
		{
			Type = _type;
			Input = _input;
			Output = _output;
			Output.Register(this);		
			_input.isConnected = true;	
		}

		public void Receive(Variable value, Input input){
			Input.Receive(value, Input);
		}

		public override void OnDestroy()
        {
			Input.isConnected = false;	
			Output.Unregister(this);
        }
	}
}
