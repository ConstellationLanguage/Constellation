namespace Constellation.Visual
{
    public class Color : INode, IReceiver
    {
        public const string NAME = "Color";
        private ISender sender;
        private Parameter valueR;
        private Parameter valueG;
        private Parameter valueB;
        private Parameter valueA;
        private Ray Result;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "R");
            _node.AddInput(this, true, "G");
            _node.AddInput(this, true, "B");
            _node.AddInput(this, true, "A");
            sender = _node.GetSender();
            _node.AddOutput(false, "Color", "Color[R][G][B][A]");
            valueR = _node.AddParameter(new Ray().Set(1), Parameter.ParameterType.ReadOnlyValueR, "R");
            valueG = _node.AddParameter(new Ray().Set(1), Parameter.ParameterType.ReadOnlyValueG, "G");
            valueB = _node.AddParameter(new Ray().Set(1), Parameter.ParameterType.ReadOnlyValueB, "B");
            valueA = _node.AddParameter(new Ray().Set(1), Parameter.ParameterType.ReadOnlyValueA, "A");
            Ray[] newVar = new Ray[4];
            newVar[0] = valueR.Value;
            newVar[1] = valueG.Value;
            newVar[2] = valueB.Value;
            newVar[3] = valueA.Value;
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
                valueR.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }
            else if (_input.InputId == 1)
            {
                valueG.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }
            else if (_input.InputId == 2)
            {
                valueB.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            } else if(_input.InputId == 3)
            {
                valueA.Value.Set(_value.GetFloat());
                Result.GetArrayVariable(_input.InputId).Set(_value.GetFloat());
            }

            if (_input.isBright){
                if(valueR.Value.GetFloat() ==  Ray.nullValue){
                    valueR.Value.Set(0);
                    Result.GetArrayVariable(0).Set(valueR.Value.GetFloat());
                }
                if(valueG.Value.GetFloat() ==  Ray.nullValue){
                    valueG.Value.Set(0);
                    Result.GetArrayVariable(1).Set(valueG.Value.GetFloat());
                }
                if(valueB.Value.GetFloat() ==  Ray.nullValue){
                    valueB.Value.Set(0);
                    Result.GetArrayVariable(2).Set(valueB.Value.GetFloat());
                }
                if(valueA.Value.GetFloat() == Ray.nullValue){
                    valueA.Value.Set(0);
                    Result.GetArrayVariable(3).Set(valueA.Value.GetFloat());
                }
                
                sender.Send(Result, 0);
            }
        }
    }
}
