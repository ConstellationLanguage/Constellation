namespace Constellation.Vectors
{
    public class SplitVec3 : INode, IReceiver {
        public const string NAME = "SplitVec3";
        private ISender sender;
        private Parameter valueX;
        private Parameter valueY;
        private Parameter valueZ;

        public void Setup (INodeParameters _node) {
            _node.AddInput (this, true, "Vec3", "Vec3");
            sender = _node.GetSender();
            _node.AddOutput (false, "X");
            _node.AddOutput (false, "Y");
            _node.AddOutput (false, "Z");
            valueX = _node.AddParameter (new Ray ().Set ("X"), Parameter.ParameterType.ReadOnlyXValue, "X");
            valueY = _node.AddParameter (new Ray ().Set ("Y"), Parameter.ParameterType.ReadOnlyYValue, "Y");
            valueZ = _node.AddParameter (new Ray ().Set ("Z"), Parameter.ParameterType.ReadOnlyZValue, "Z");
            Ray[] newVar = new Ray[3];
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

        public void Receive (Ray _value, Input _input) {
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

            if (_input.isBright) {
                sender.Send (valueX.Value, 0);
                sender.Send (valueY.Value, 1);
                sender.Send (valueZ.Value, 2);
            }
        }
    }
}