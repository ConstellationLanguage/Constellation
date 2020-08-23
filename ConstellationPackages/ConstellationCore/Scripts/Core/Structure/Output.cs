using System.Collections.Generic;

namespace Constellation
{
	[System.Serializable]
	public class Output: IReceiver
	{
		public List<IReceiver> Receivers;
		public string Guid;
		public bool IsBright;
		public string Type;
		public string Description;

		public Output (string _guid, bool _isBright, string _type, string _description)
		{
			Guid = _guid;
			IsBright = _isBright;
			Type = _type;
			Description = _description;
		}

		public void Register(IReceiver _receiver)
		{
			if(Receivers == null)
				Receivers = new List<IReceiver>();

			Receivers.Add(_receiver);
		}

		public void Unregister(IReceiver _receiver)
		{
			if(Receivers != null)
				Receivers.Remove(_receiver);
		}

		public void Receive(Ray Value, Input input)
		{
			Send (Value);
		}
		
		public void Send(Ray value)
		{
			if(Receivers == null)
				return;

			foreach(IReceiver receiver in Receivers)
			{
				receiver.Receive (value, null);
			}
		}
	}
}
