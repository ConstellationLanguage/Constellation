using UnityEngine;

namespace Constellation.UI
{
    public class Image : INode, IReceiver, IRequireGameObject
    {
        UnityEngine.UI.Image image;
        public const string NAME = "Image";
        private Ray ColorVar;
        ISender sender;

        public void Setup(INodeParameters _nodeParameters)
        {
            sender = _nodeParameters.GetSender();

            _nodeParameters.AddInput(this, false, "Object", "Image component");
            _nodeParameters.AddInput(this, false, "Object", "Sprite");
            _nodeParameters.AddInput(this, false, "Color", "Image tint");
            _nodeParameters.AddInput(this, true, "Any", "Get imageValues");

            Ray[] newColorVar = new Ray[4];
            newColorVar[0] = new Ray().Set(0);
            newColorVar[1] = new Ray().Set(0);
            newColorVar[2] = new Ray().Set(0);
            newColorVar[3] = new Ray().Set(0);
            ColorVar = new Ray().Set(newColorVar);

            _nodeParameters.AddOutput(false, "Object", "Sprite");
            _nodeParameters.AddOutput(false, "Color", "Image tint");
        }

        void UpdateImage()
        {

        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Set(GameObject _gameObject)
        {
            var newImage = _gameObject.GetComponent<UnityEngine.UI.Image>();
            if (newImage != null)
            {
                image = newImage;
            }
        }

        public void Receive(Ray value, Input _input)
        {
            if (_input.InputId == 0)
            {
                Set(UnityObjectsConvertions.ConvertToGameObject(value.GetObject()));

            }
            else if (_input.InputId == 1 && image != null)
            {
                var sprite = UnityObjectsConvertions.ConvertToSprite(value);
                if (sprite != null && image != null)
                {
                    image.sprite = sprite;
                }
            }
            else if (_input.InputId == 2 && image != null)
            {
                ColorVar.Set(value.GetArray());
                image.color = new Color(ColorVar.GetArrayVariable(0).GetFloat(), ColorVar.GetArrayVariable(1).GetFloat(), ColorVar.GetArrayVariable(2).GetFloat(), ColorVar.GetArrayVariable(3).GetFloat());
            }
            else if (_input.isBright && image != null)
            {
                Ray[] newVar = new Ray[4];
                newVar[0] = new Ray().Set(image.color.r);
                newVar[1] = new Ray().Set(image.color.g);
                newVar[2] = new Ray().Set(image.color.b);
                newVar[3] = new Ray().Set(image.color.a);
                sender.Send(new Ray().Set(image.sprite), 0);
                sender.Send(new Ray().Set(new Ray().Set(newVar)), 1);
            }
        }
    }
}