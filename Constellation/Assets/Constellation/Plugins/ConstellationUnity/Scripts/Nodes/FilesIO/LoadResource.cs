using System.IO;
using UnityEngine;

namespace Constellation.FilesIO
{
    public class LoadResource : INode, IReceiver
    {
        public const string NAME = "LoadResource";
        private ISender sender;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "File path");
            _node.AddOutput(false, "Object","The resource");
            sender = _node.GetSender();
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
                var resource = Resources.Load(_value.GetString());
                sender.Send(new Ray().Set(resource), 0);
            }
        }
    }
}
