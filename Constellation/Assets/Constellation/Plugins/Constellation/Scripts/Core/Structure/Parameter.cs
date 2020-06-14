namespace Constellation {

    [System.Serializable]
    public class Parameter {
        public delegate void OnAttributeChanged();

        public enum ParameterType {
            Value, 
            Word, 
            NoteField, 
            Conditionals, Then, Else, 
            ReadOnlyValue, 
            ReadOnlyXValue, ReadOnlyYValue, ReadOnlyZValue, 
            ReadOnlyValueR, ReadOnlyValueG, ReadOnlyValueB, ReadOnlyValueA,
            RenameNodeTitle
        };

        public Ray Value;
        public ParameterType Type;
        public const int ParameterInputID = 1000;
        public Parameter (ParameterType _type) {
            Value = new Ray ();
            Value.Set (0);
            Type = _type;
        }
    }
}