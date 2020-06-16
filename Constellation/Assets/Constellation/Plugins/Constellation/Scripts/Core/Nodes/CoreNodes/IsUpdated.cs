namespace Constellation.CoreNodes
{
	[System.Serializable]
	public class IsUpdated : INode, IReceiver
	{
		public const string NAME = "IsUpdated";
		private ISender sender;
		private bool wasUpdated = false;

		public void Setup(INodeParameters _node)
		{
			sender = _node.GetSender();
			_node.AddOutput(false, "1 if updated 0 if not");
			_node.AddInput(this,false, "Any","Check if node was updated since last frame");
			_node.AddInput(this,true, "Any", "The updateCheck");
		}

		public string NodeName()
		{
			return NAME;
		}

		public string NodeNamespace()
		{
			return NameSpace.NAME;
		}

		public void Receive(Ray _value, Input _input)
		{
			if (_input.InputId == 0)
			{
				wasUpdated = true;

			}
			else if(_input.InputId == 1)
			{
				if (wasUpdated)
				{
					sender.Send(new Ray(1), 0);
					wasUpdated = false;
				} else
				{
					sender.Send(new Ray(0), 0);
				}
			}
			
		}

		public void OnUpdate()
		{
			

		}
	}
}