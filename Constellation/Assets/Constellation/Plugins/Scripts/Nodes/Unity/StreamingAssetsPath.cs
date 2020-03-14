using System.IO;
using UnityEngine;

namespace Constellation.Unity
{
    public class StreamingAssetsPath : INode, IReceiver, IAwakable
    {
        public const string NAME = "StreamingAssetsPath";
        private ISender sender;

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender();
            _node.AddOutput(false, "Path");
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
        }

        public void OnAwake()
        {
            sender.Send(new Variable(Application.streamingAssetsPath), 0);
        }
    }
}
