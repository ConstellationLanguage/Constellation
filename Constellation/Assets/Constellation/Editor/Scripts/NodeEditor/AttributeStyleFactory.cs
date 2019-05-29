using Constellation;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public static class AttributeStyleFactory {
        public static Variable Draw (Attribute.AttributeType type, Rect size, Variable Value) {

            switch (type) {
                case Attribute.AttributeType.Value:
                    return Value.Set (EditorGUI.FloatField (size, "<>", Value.GetFloat ()));
                case Attribute.AttributeType.Word:
                    return Value.Set (EditorGUI.TextField (size, "", Value.GetString ()));
                case Attribute.AttributeType.Conditionals:
                    return IfCharacterFilter (size, Value);
                case Attribute.AttributeType.Then:
                    return ThenCharacterFilter (size, Value);
                case Attribute.AttributeType.Else:
                    return ElseCharacterFilter (size, Value);
                case Attribute.AttributeType.NoteField:
                    return Value.Set (EditorGUI.TextArea (new Rect (0, 20, 120, 100), Value.GetString (), GUI.skin.GetStyle ("VCS_StickyNote")));
                case Attribute.AttributeType.ReadOnlyValue:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyXValue:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyYValue:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyZValue:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueR:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueG:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueB:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueA:
                    EditorGUI.LabelField (size, Value.GetString ());
                    return Value;
                case Attribute.AttributeType.RenameNodeTitle:
                    EditorGUI.LabelField(size, Value.GetString());
                    return Value;
                default:
                    return Value;
            }
        }

        public static Variable Reset (Attribute.AttributeType type, Variable Value) {

            switch (type) {
                case Attribute.AttributeType.Value:
                    return Value;
                case Attribute.AttributeType.Word:
                    return Value.Set (Value.GetString ());
                case Attribute.AttributeType.Conditionals:
                    return Value.Set (Value.GetString ());
                case Attribute.AttributeType.Then:
                    return Value.Set (Value.GetString ());
                case Attribute.AttributeType.Else:
                    return Value.Set (Value.GetString ());
                case Attribute.AttributeType.NoteField:
                    return Value.Set (Value.GetString ());
                case Attribute.AttributeType.ReadOnlyValue:
                    Value.Set (0);
                    return Value;
                case Attribute.AttributeType.ReadOnlyXValue:
                    Value.Set ("X");
                    return Value;
                case Attribute.AttributeType.ReadOnlyYValue:
                    Value.Set ("Y");
                    return Value;
                case Attribute.AttributeType.ReadOnlyZValue:
                    Value.Set ("Z");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueR:
                    Value.Set ("R");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueG:
                    Value.Set ("G");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueB:
                    Value.Set ("B");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueA:
                    Value.Set ("A");
                    return Value;
                default:
                    return Value;
            }
        }

        private static Variable IfCharacterFilter (Rect size, Variable Value) {
            return Value.Set (Regex.Replace (EditorGUI.TextField (size, "if", Value.GetString ()), "[a-zA-Z ]", ""));
        }

        private static Variable ThenCharacterFilter (Rect size, Variable Value) {
            return Value.Set (Regex.Replace (EditorGUI.TextField (size, "then", Value.GetString ()), "[a-zA-Z ]", ""));
        }

        private static Variable ElseCharacterFilter (Rect size, Variable Value) {
            return Value.Set (Regex.Replace (EditorGUI.TextField (size, "else", Value.GetString ()), "[a-zA-Z ]", ""));
        }
    }
}