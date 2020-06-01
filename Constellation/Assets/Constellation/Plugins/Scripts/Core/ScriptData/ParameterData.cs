
namespace Constellation
{
    [System.Serializable]
    public class ParameterData
    {
        public Ray Value;
        public Parameter.ParameterType Type;

        public ParameterData(Parameter.ParameterType _type)
        {
            Value = new Ray();
            Value.Set(0);
            Type = _type;
        }

        public ParameterData(Parameter.ParameterType _type, Ray variable)
        {
            if(_type == Parameter.ParameterType.Value)
                Value = new Ray().Set(variable.GetFloat());
            else if(_type == Parameter.ParameterType.Word)
                 Value = new Ray().Set(variable.GetString());
            else 
                Value = new Ray().Set(variable.GetString());
                
            Type = _type;
        }
    }
}
