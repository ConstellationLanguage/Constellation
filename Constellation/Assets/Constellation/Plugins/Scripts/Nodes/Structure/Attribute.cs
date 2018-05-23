namespace Constellation {

    [System.Serializable]
    public class Attribute {
        public enum AttributeType { Value, 
            Word, 
            NoteField, 
            Conditionals, Then, Else, 
            ReadOnlyValue, 
            ReadOnlyXValue, ReadOnlyYValue, ReadOnlyZValue, 
            ReadOnlyValueR, ReadOnlyValueG, ReadOnlyValueB, ReadOnlyValueA
        };
        public Variable Value;
        public AttributeType Type;

        public Attribute (AttributeType _type) {
            Value = new Variable ();
            Value.Set (0);
            Type = _type;
        }
    }
}