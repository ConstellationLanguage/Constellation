namespace Constellation.Unity {
    public class SplitVec3 : INode, IReceiver {
        public const string NAME = "SplitVec3";
        private ISender sender;
        private Attribute valueX;
        private Attribute valueY;
        private Attribute valueZ;

        public void Setup (INodeParameters _node) {
            _node.AddInput (this, true, "Vec3");
            sender = _node.GetSender();
            _node.AddOutput (false, "X");
            _node.AddOutput (false, "Y");
            _node.AddOutput (false, "Z");
            valueX = _node.AddAttribute (new Variable ().Set ("X"), Attribute.AttributeType.ReadOnlyXValue, "X");
            valueY = _node.AddAttribute (new Variable ().Set ("Y"), Attribute.AttributeType.ReadOnlyYValue, "Y");
            valueZ = _node.AddAttribute (new Variable ().Set ("Z"), Attribute.AttributeType.ReadOnlyZValue, "Z");
            Variable[] newVar = new Variable[3];
            newVar[0] = valueX.Value;
            newVar[1] = valueY.Value;
            newVar[2] = valueZ.Value;
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable _value, Input _input) {
            if (_input.InputId == 0) {
                if(_value.GetObject() == null) {
                    valueX.Value.Set (_value.GetFloat (0));
                    valueY.Value.Set (_value.GetFloat (1));
                    valueZ.Value.Set (_value.GetFloat (2));
                } else if(_value.GetObject() != null){
                    var vector = UnityObjectsConvertions.ConvertToVector3(_value.GetObject());
                    valueX.Value.Set (vector.x);
                    valueY.Value.Set (vector.y);
                    valueZ.Value.Set (vector.z);
                }
            }

            if (_input.isWarm) {
                sender.Send (valueX.Value, 0);
                sender.Send (valueY.Value, 1);
                sender.Send (valueZ.Value, 2);
            }
        }
    }
}