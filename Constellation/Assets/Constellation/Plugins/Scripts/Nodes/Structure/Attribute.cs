namespace Constellation {

    [System.Serializable]
    public class Parameter {
        public delegate void OnAttributeChanged();

        public enum AttributeType {
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
        public AttributeType Type;

        public Parameter (AttributeType _type) {
            Value = new Ray ();
            Value.Set (0);
            Type = _type;
        }
    }
}