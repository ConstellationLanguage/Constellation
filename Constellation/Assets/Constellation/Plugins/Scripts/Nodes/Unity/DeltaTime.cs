using UnityEngine;

namespace Constellation.Unity
{
    public class DeltaTime : INode, IReceiver
    {
        public const string NAME = "DeltaTime";
        private ISender sender;
        private Variable deltaTimeVar;

        public void Setup(INodeParameters _nodeParameters)
        {
            _nodeParameters.AddInput(this, true, "Value");
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(false, "Value x Delta time");
            deltaTimeVar = new Variable().Set(0);
        }

        public string NodeName() {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            deltaTimeVar.Set(_value.GetFloat() * Time.deltaTime);
            sender.Send(deltaTimeVar, 0);
        }
    }
}
