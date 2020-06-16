using System.IO;
using UnityEngine;

namespace Constellation.FilesIO
{
    public class LoadTextFileAtPath : INode, IReceiver
    {
        public const string NAME = "LoadTextFileAtPath";
        private ISender sender;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "File path");
            _node.AddOutput(false, "The text");
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
                //
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_value.GetString());
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                string jsonString = www.downloadHandler.text;
                sender.Send(new Ray().Set(jsonString), 0);
            }
        }
    }
}
