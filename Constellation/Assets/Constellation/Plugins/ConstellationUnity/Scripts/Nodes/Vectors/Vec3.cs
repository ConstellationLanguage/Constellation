namespace Constellation.Vectors
{
    public class Vec3 : INode, IReceiver
    {
        public const string NAME = "Vec3";
        private ISender sender;
        private Parameter valueX;
        private Parameter valueY;
        private Parameter valueZ;
        private Ray Result;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "X");
            _node.AddInput(this, true, "Y");
            _node.AddInput(this, true, "Z");
            sender = _node.GetSender();
            _node.AddOutput(false, "Vec3", "Vec3[X][Y][Z]");
            valueX = _node.AddParameter(new Ray().Set(0), Parameter.ParameterType.ReadOnlyXValue, "X");
            valueY = _node.AddParameter(new Ray().Set(0), Parameter.ParameterType.ReadOnlyYValue, "Y");
            valueZ = _node.AddParameter(new Ray().Set(0), Parameter.ParameterType.ReadOnlyZValue, "Z");
            Ray[] newVar = new Ray[3];
            newVar[0] = valueX.Value;
            newVar[1] = valueY.Value;
            newVar[2] = valueZ.Value;
            Result = new Ray().Set(newVar);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            if (_input.InputId == 0)
            {
                valueX.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }
            else if (_input.InputId == 1)
            {
                valueY.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }
            else if (_input.InputId == 2)
            {
                valueZ.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }

            if (_input.isBright){
                if(valueX.Value.GetFloat() ==  Ray.nullValue){
                    valueX.Value.Set(0);
                    Result.GetArrayVariable(0).Set(valueX.Value.GetFloat());
                }
                if(valueY.Value.GetFloat() ==  Ray.nullValue){
                    valueY.Value.Set(0);
                    Result.GetArrayVariable(1).Set(valueY.Value.GetFloat());
                }
                if(valueZ.Value.GetFloat() ==  Ray.nullValue){
                    valueZ.Value.Set(0);
                    Result.GetArrayVariable(2).Set(valueZ.Value.GetFloat());
                }
                
                sender.Send(Result, 0);
            }
        }
    }
}
