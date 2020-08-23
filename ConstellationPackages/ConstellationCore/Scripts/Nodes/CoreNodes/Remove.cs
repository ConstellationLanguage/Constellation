namespace Constellation.CoreNodes {
    public class Remove : INode, IReceiver {
        private ISender sender;
        private Ray[] varsToRemove;
        private Ray result;
        public const string NAME = "Remove";
        public void Setup (INodeParameters node) {
            node.AddInput (this, false, "Value to remove");
            node.AddInput (this, true, "Remove factor");
            sender = node.GetSender();
            node.AddOutput (false, "$1 - $2");
            varsToRemove = new Ray[2];
            varsToRemove[0] = new Ray ().Set (0);
            varsToRemove[1] = new Ray ().Set (0);
            result = new Ray ().Set (0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray _value, Input _input) {
            if (_value.IsFloat ())
                varsToRemove[_input.InputId].Set (_value.GetFloat ());
            else
                varsToRemove[_input.InputId].Set (_value.GetString ());

            if (varsToRemove[0].IsFloat () && varsToRemove[1].IsFloat () && _input.isBright)
                result.Set (varsToRemove[0].GetFloat () - varsToRemove[1].GetFloat ());
            else if (_input.isBright)
                result.Set (varsToRemove[0].GetString ().Replace (varsToRemove[1].GetString (), ""));

            if (_input.isBright)
                sender.Send (result, 0);
        }
    }
}