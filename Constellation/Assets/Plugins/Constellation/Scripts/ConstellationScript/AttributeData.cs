
namespace Constellation
{
    [System.Serializable]
    public class AttributeData
    {
        public Variable Value;
        public Attribute.AttributeType Type;

        public AttributeData(Attribute.AttributeType _type)
        {
            Value = new Variable();
            Value.Set(0);
            Type = _type;
        }

        public AttributeData(Attribute.AttributeType _type, Variable variable)
        {
            if(_type == Attribute.AttributeType.Value)
                Value = new Variable().Set(variable.GetFloat());
            else if(_type == Attribute.AttributeType.Word)
                 Value = new Variable().Set(variable.GetString());
            else 
                Value = new Variable().Set(variable.GetString());
                
            Type = _type;
        }
    }
}
