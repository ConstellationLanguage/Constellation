namespace Constellation {

    [System.Serializable]
    public class Attribute {
<<<<<<< HEAD
        public enum AttributeType { Value, Word, NoteField, Conditionals, Then, Else, ReadOnlyValue, ReadOnlyXValue, ReadOnlyYValue, ReadOnlyZValue };
        public Variable Value;
        public AttributeType Type;
=======
        public enum AttributeType { Value, 
        Word, 
        NoteField, 
        Conditionals, Then, Else, 
        ReadOnlyValue, 
        ReadOnlyXValue, ReadOnlyYValue, ReadOnlyZValue, 
        ReadOnlyValueR, ReadOnlyValueG, ReadOnlyValueB, ReadOnlyValueA };
 public Variable Value;
 public AttributeType Type;
>>>>>>> e459bba71f871cae96fbb70339d360abf83f682e

        public Attribute (AttributeType _type) {
            Value = new Variable ();
            Value.Set (0);
            Type = _type;
        }
    }
}