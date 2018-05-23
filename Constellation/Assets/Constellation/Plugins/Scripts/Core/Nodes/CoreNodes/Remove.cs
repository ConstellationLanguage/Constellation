namespace Constellation.CoreNodes {
    public class Remove : INode, IReceiver {
        private ISender sender;
        private Variable[] varsToRemove;
        private Variable result;
        public const string NAME = "Remove";
        public void Setup (INodeParameters node) {
            node.AddInput (this, false, "Value to remove");
            node.AddInput (this, true, "Remove factor");
            sender = node.GetSender();
            node.AddOutput (false, "$1 - $2");
            varsToRemove = new Variable[2];
            varsToRemove[0] = new Variable ().Set (0);
            varsToRemove[1] = new Variable ().Set (0);
            result = new Variable ().Set (0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable _value, Input _input) {
            if (_value.IsFloat ())
                varsToRemove[_input.InputId].Set (_value.GetFloat ());
            else
                varsToRemove[_input.InputId].Set (_value.GetString ());

            if (varsToRemove[0].IsFloat () && varsToRemove[1].IsFloat () && _input.isWarm)
                result.Set (varsToRemove[0].GetFloat () - varsToRemove[1].GetFloat ());
            else if (_input.isWarm)
                result.Set (varsToRemove[0].GetString ().Replace (varsToRemove[1].GetString (), ""));

            if (_input.isWarm)
                sender.Send (result, 0);
        }
    }
}