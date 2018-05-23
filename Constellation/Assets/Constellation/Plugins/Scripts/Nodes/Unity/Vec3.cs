namespace Constellation.Unity
{
    public class Vec3 : INode, IReceiver
    {
        public const string NAME = "Vec3";
        private ISender sender;
        private Attribute valueX;
        private Attribute valueY;
        private Attribute valueZ;
        private Variable Result;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "X");
            _node.AddInput(this, true, "Y");
            _node.AddInput(this, true, "Z");
            sender = _node.GetSender();
            _node.AddOutput(false, "Vec3[X][Y][Z]");
            valueX = _node.AddAttribute(new Variable().Set(0), Attribute.AttributeType.ReadOnlyXValue, "X");
            valueY = _node.AddAttribute(new Variable().Set(0), Attribute.AttributeType.ReadOnlyYValue, "Y");
            valueZ = _node.AddAttribute(new Variable().Set(0), Attribute.AttributeType.ReadOnlyZValue, "Z");
            Variable[] newVar = new Variable[3];
            newVar[0] = valueX.Value;
            newVar[1] = valueY.Value;
            newVar[2] = valueZ.Value;
            Result = new Variable().Set(newVar);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
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

            if (_input.isWarm){
                if(valueX.Value.GetFloat() ==  Variable.nullValue){
                    valueX.Value.Set(0);
                    Result.GetArrayVariable(0).Set(valueX.Value.GetFloat());
                }
                if(valueY.Value.GetFloat() ==  Variable.nullValue){
                    valueY.Value.Set(0);
                    Result.GetArrayVariable(1).Set(valueY.Value.GetFloat());
                }
                if(valueZ.Value.GetFloat() ==  Variable.nullValue){
                    valueZ.Value.Set(0);
                    Result.GetArrayVariable(2).Set(valueZ.Value.GetFloat());
                }
                
                sender.Send(Result, 0);
            }
        }
    }
}
