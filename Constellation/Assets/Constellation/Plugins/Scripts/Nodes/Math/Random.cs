namespace Constellation.Math {
public class Random: INode, IReceiver
    {
		private ISender sender;
		private Ray VarF;
		private Ray VarP;
        public const string NAME = "Random";
        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, false, "Start");
			_node.AddInput(this, true, "End");
			VarF = new Ray(0);
			VarP = new Ray(0);
            sender = _node.GetSender();
            _node.AddOutput(false, "Random Value between Start and End");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
			if(_input.InputId == 0)
				VarF.Set(_value.GetFloat());
			
			if(_input.InputId == 1)
				VarP.Set(_value.GetFloat());

            if (_input.isBright)
                sender.Send(new Ray().Set(UnityEngine.Random.Range(VarF.GetFloat(), VarP.GetFloat())), 0);
        }
    }
}
