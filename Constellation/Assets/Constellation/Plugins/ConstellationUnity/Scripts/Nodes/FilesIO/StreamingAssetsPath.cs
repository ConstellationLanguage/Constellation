using System.IO;
using UnityEngine;

namespace Constellation.FilesIO
{
    public class StreamingAssetsPath : INode, IReceiver, IAwakable
    {
        public const string NAME = "StreamingAssetsPath";
        private ISender sender;

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender();
            _node.AddOutput(true, "Path");
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
        }

        public void OnAwake()
        {
            string filePath = Application.streamingAssetsPath;
            sender.Send(new Ray(filePath+ "/"), 0);
        }
    }
}
