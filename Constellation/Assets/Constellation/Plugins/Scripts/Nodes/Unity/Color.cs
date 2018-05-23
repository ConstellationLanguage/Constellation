namespace Constellation.Unity
{
    public class Color : INode, IReceiver
    {
        public const string NAME = "Color";
        private ISender sender;
        private Attribute valueR;
        private Attribute valueG;
        private Attribute valueB;
        private Attribute valueA;
        private Variable Result;

        public void Setup(INodeParameters _node)
        {
            _node.AddInput(this, true, "R");
            _node.AddInput(this, true, "G");
            _node.AddInput(this, true, "B");
            _node.AddInput(this, true, "A");
            sender = _node.GetSender();
            _node.AddOutput(false, "Color[R][G][B][A]");
            valueR = _node.AddAttribute(new Variable().Set(1), Attribute.AttributeType.ReadOnlyValueR, "R");
            valueG = _node.AddAttribute(new Variable().Set(1), Attribute.AttributeType.ReadOnlyValueG, "G");
            valueB = _node.AddAttribute(new Variable().Set(1), Attribute.AttributeType.ReadOnlyValueB, "B");
            valueA = _node.AddAttribute(new Variable().Set(1), Attribute.AttributeType.ReadOnlyValueA, "A");
            Variable[] newVar = new Variable[4];
            newVar[0] = valueR.Value;
            newVar[1] = valueG.Value;
            newVar[2] = valueB.Value;
            newVar[3] = valueA.Value;
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

            if (_input.isWarm){
                if(valueR.Value.GetFloat() ==  Variable.nullValue){
                    valueR.Value.Set(0);
                    Result.GetArrayVariable(0).Set(valueR.Value.GetFloat());
                }
                if(valueG.Value.GetFloat() ==  Variable.nullValue){
                    valueG.Value.Set(0);
                    Result.GetArrayVariable(1).Set(valueG.Value.GetFloat());
                }
                if(valueB.Value.GetFloat() ==  Variable.nullValue){
                    valueB.Value.Set(0);
                    Result.GetArrayVariable(2).Set(valueB.Value.GetFloat());
                }
                if(valueA.Value.GetFloat() == Variable.nullValue){
                    valueA.Value.Set(0);
                    Result.GetArrayVariable(3).Set(valueA.Value.GetFloat());
                }
                
                sender.Send(Result, 0);
            }
        }
    }
}
