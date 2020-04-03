
namespace Constellation
{
    [System.Serializable]
    public class AttributeData
    {
        public Ray Value;
        public Parameter.AttributeType Type;

        public AttributeData(Parameter.AttributeType _type)
        {
            Value = new Ray();
            Value.Set(0);
            Type = _type;
        }

        public AttributeData(Parameter.AttributeType _type, Ray variable)
        {
            if(_type == Parameter.AttributeType.Value)
                Value = new Ray().Set(variable.GetFloat());
            else if(_type == Parameter.AttributeType.Word)
                 Value = new Ray().Set(variable.GetString());
            else 
                Value = new Ray().Set(variable.GetString());
                
            Type = _type;
        }
    }
}
