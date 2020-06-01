namespace Constellation.CoreNodes
{
    public class BrightExit : INode, IReceiver, IMirrorNode, IBrightExitNode, IExitNode
    {
        private ISender sender;
        private Parameter parameter; // attributes are setted in the editor.
        public const string NAME = "BrightExit"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        private Ray exitValue;
        IRayReceiver receiver;
        private int outputId;
        public void Setup(INodeParameters _node)
        {
            var wordValue = new Ray();
            _node.AddInput(this, true, "Any", "Output the entry received"); // setting a cold input
            parameter = _node.AddParameter(wordValue.Set("Var"), Parameter.ParameterType.Word, "The default word");// setting an attribute (Used only for the editor)
            exitValue = new Ray();
        }

        //return the node name (used in the factory).
        public string NodeName()
        {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive(Ray _value, Input _input)
        {
            exitValue = new Ray(_value);
            receiver.SendRay(exitValue, outputId);
        }

        public Ray GetExitValue()
        {
            return exitValue;
        }

        public void SubscribeReceiver(IRayReceiver receiver, int id)
        {
            this.receiver = receiver;
            outputId = id;
        }
    }
}